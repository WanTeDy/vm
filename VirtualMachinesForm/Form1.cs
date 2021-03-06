﻿using Microsoft.Azure;
using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using System.Web.Script.Serialization;
using VirtualMachinesForm.Helpers;
using System.Diagnostics;

namespace VirtualMachinesForm
{
    public partial class Form1 : Form
    {
        private string VMName = "SimpleWinVM";
        private string subscriptionId = "a755ef57-8bdd-447e-bd18-9f89ae802903";
        private string deploymentName = "MyDeployment";
        private string location = "UKSouth";
        private bool shownModal = false;
        private static ApplicationParametersModel appParameters { get; set; }
        private static List<VMModel> resources;
        private TokenCredentials credential;
        private JavaScriptSerializer serializer;

        public static List<VMModel> Resources { get { return resources = resources ?? new List<VMModel>(); } }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serializer = new JavaScriptSerializer();
            try
            {
                resources = serializer.Deserialize<List<VMModel>>(FileHelper.GetString());
                appParameters = serializer.Deserialize<ApplicationParametersModel>(FileHelper.GetString("application.json"));
            }
            catch (Exception exp)
            {
                Trace.TraceInformation("Failed deserialization resources and application params");
                TraceHelper.TraceException(exp);
                MessageBox.Show("Файл с параметрами не найден или поврежден", "VMManager", MessageBoxButtons.OK);
            }
            UpdateData();
            SetupButtonsState();
            ThreadPool.QueueUserWorkItem((x) => DeleteAllStorageVHD());
            ThreadPool.QueueUserWorkItem((x) => SaveGroupStates());
            ThreadPool.QueueUserWorkItem((x) => SaveGroupStatesReserved());
        }

        private async void UpdateCredentials()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                credential = new TokenCredentials(token.AccessToken);
            }
            catch (Exception exp)
            {
                if (!shownModal)
                {
                    Trace.TraceInformation("Failed update credentials");
                    TraceHelper.TraceException(exp);
                    shownModal = true;
                    MessageBox.Show("ApplicationId неправильный или не имеет доступа! Либо нет подключения к интернету", "VMManager", MessageBoxButtons.OK);
                    shownModal = false;
                }
            }
        }

        private void UpdateData()
        {
            //Thread threadCredentials = new Thread(UpdateCredentials);
            Thread thread = new Thread(UpdateGrid);

            thread.IsBackground = /*threadCredentials.IsBackground =*/ true;

            //threadCredentials.Start();
            thread.Start();
        }

        private async void UpdateGrid(object obj)
        {
            while (true)
            {
                UpdateCredentials();
                if (Resources != null)
                {
                    foreach (var item in Resources.ToList())
                    {
                        try
                        {
                            item.VMStatus = await GetVirtualMachineStatusAsync(credential, item.ResourceGroupName, VMName, subscriptionId);
                        }
                        catch
                        {
                            //Resources.Remove(item);
                            item.VMStatus = "VM not available";
                        }
                    }
                    dataGridView.Invoke(new Action(() =>
                    {
                        var row = 0;
                        var col = 0;
                        if (dataGridView.SelectedCells.Count > 0)
                        {
                            row = dataGridView.SelectedCells[0].RowIndex;
                            col = dataGridView.SelectedCells[0].ColumnIndex;
                        }
                        dataGridView.DataSource = Resources.ToList();
                        //dataGridView.Refresh();
                        if (dataGridView.SelectedCells.Count > 0 && dataGridView.Rows.Count > row && dataGridView.ColumnCount > col)
                        {
                            dataGridView.Rows[row].Cells[col].Selected = true;
                        }
                    }));
                    //SaveGroupStates();
                }
                Thread.Sleep(3000);
            }
        }

        private void SaveGroupStatesReserved()
        {
            Thread.Sleep(12000);
            try
            {
                string path = Directory.GetCurrentDirectory() + "resource_copy.txt";
                FileHelper.SaveString(serializer.Serialize(Resources), path);
            }
            catch (Exception exp)
            {
                Trace.TraceInformation("Failed save resources_copy");
                TraceHelper.TraceException(exp);
            }
        }

        private void SaveGroupStates()
        {
            try
            {
                FileHelper.SaveString(serializer.Serialize(Resources));
            }
            catch (Exception exp)
            {
                Trace.TraceInformation("Failed save resources");
                TraceHelper.TraceException(exp);
                Thread.Sleep(6000);
            }
        }

        //Получение доступа к ажуру
        private static async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            var cc = new ClientCredential(appParameters.ApplicationId, appParameters.ApplicationSecret);
            var context = new AuthenticationContext("https://login.windows.net/aa05c4a5-1597-45ee-8470-42adb07f7817", null);
            return await context.AcquireTokenAsync("https://management.azure.com/", cc);
        }

        //Создание группы ресурсов
        public static async Task<ResourceGroup> CreateResourceGroupAsync(
            TokenCredentials credential,
            string groupName,
            string subscriptionId,
            string location)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };

            var resourceGroup = new ResourceGroup { Location = location };
            return await resourceManagementClient.ResourceGroups.CreateOrUpdateAsync(
              groupName,
              resourceGroup).ConfigureAwait(false);
        }

        //Создание набора ресурсов для деплоя
        public static async Task<DeploymentExtended> CreateTemplateDeploymentAsync(
            TokenCredentials credential,
            string groupName,
            string deploymentName,
            string subscriptionId)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };

            var deployment = new Deployment();
            deployment.Properties = new DeploymentProperties
            {
                Mode = DeploymentMode.Incremental,
                //TemplateLink = new TemplateLink("https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/azuredeploy.json"),
                Template = File.ReadAllText("..\\GlobalParameterstest4.json").Replace("osDiskblob", "osDisk-" + groupName),
                //Template = File.ReadAllText("..\\GlobalParameters.json"),
                Parameters = File.ReadAllText("..\\Parameters.json").Replace("mydns", groupName)
            };
            return await resourceManagementClient.Deployments.CreateOrUpdateAsync(
              groupName,
              deploymentName,
              deployment).ConfigureAwait(false);
        }

        //Удаление всей группы ресурсов
        public static async void DeleteResourceGroupAsync(
            TokenCredentials credential,
            string groupName,
            string subscriptionId)
        {
            var resourceManagementClient = new ResourceManagementClient(credential)
            { SubscriptionId = subscriptionId };
            await resourceManagementClient.ResourceGroups.DeleteAsync(groupName);
        }

        //Удаление диска
        public static void DeleteStorageVHD(string groupName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("vmcontainerd7962b02-629a-468a-a901-266a6e53cf77");
            // Loop over items within the container and output the length and URI.
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("osDisk-" + groupName + "-copy.d7962b02-629a-468a-a901-266a6e53cf77.vhd");

            // Delete the blob.
            blockBlob.Delete();

            //foreach (IListBlobItem item in container.ListBlobs(null, false))
            //{
            //    CloudPageBlob pageBlob = item as CloudPageBlob;
            //    if (pageBlob != null)
            //    {
            //        try
            //        {
            //            pageBlob.DeleteAsync();
            //        }
            //        catch { }
            //    }
            //}
        }

        //Удаление всех дисков
        public static void DeleteAllStorageVHD()
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("vmcontainerd7962b02-629a-468a-a901-266a6e53cf77");
                // Loop over items within the container and output the length and URI.

                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    CloudPageBlob pageBlob = item as CloudPageBlob;
                    if (pageBlob != null)
                    {
                        try
                        {
                            pageBlob.Delete();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception exp)
            {
                Trace.TraceInformation("Failed deleting storage VHD's");
                TraceHelper.TraceException(exp);
            }
            Thread.Sleep(60000);
        }

        //Получение информации о виртуальной машине
        public static async Task<string> GetVirtualMachineStatusAsync(
            TokenCredentials credential,
            string groupName,
            string vmName,
            string subscriptionId)
        {
            var computeManagementClient = new ComputeManagementClient(credential)
            { SubscriptionId = subscriptionId };
            var vmResult = await computeManagementClient.VirtualMachines.GetAsync(
              groupName,
              vmName,
              InstanceViewTypes.InstanceView);
            //Console.WriteLine("hardwareProfile");
            //Console.WriteLine("   vmSize: " + vmResult.HardwareProfile.VmSize);

            //Console.WriteLine("\nstorageProfile");
            //Console.WriteLine("  imageReference");
            //Console.WriteLine("    publisher: " + vmResult.StorageProfile.ImageReference.Publisher);
            //Console.WriteLine("    offer: " + vmResult.StorageProfile.ImageReference.Offer);
            //Console.WriteLine("    sku: " + vmResult.StorageProfile.ImageReference.Sku);
            //Console.WriteLine("    version: " + vmResult.StorageProfile.ImageReference.Version);
            //Console.WriteLine("  osDisk");
            //Console.WriteLine("    osType: " + vmResult.StorageProfile.OsDisk.OsType);
            //Console.WriteLine("    name: " + vmResult.StorageProfile.OsDisk.Name);
            //Console.WriteLine("    createOption: " + vmResult.StorageProfile.OsDisk.CreateOption);
            //Console.WriteLine("    uri: " + vmResult.StorageProfile.OsDisk.Vhd.Uri);
            //Console.WriteLine("    caching: " + vmResult.StorageProfile.OsDisk.Caching);
            //Console.WriteLine("\nosProfile");
            //Console.WriteLine("  computerName: " + vmResult.OsProfile.ComputerName);
            //Console.WriteLine("  adminUsername: " + vmResult.OsProfile.AdminUsername);
            //Console.WriteLine("  provisionVMAgent: " + vmResult.OsProfile.WindowsConfiguration.ProvisionVMAgent.Value);
            //Console.WriteLine("  enableAutomaticUpdates: " + vmResult.OsProfile.WindowsConfiguration.EnableAutomaticUpdates.Value);

            //Console.WriteLine("\nnetworkProfile");
            //foreach (NetworkInterfaceReference nic in vmResult.NetworkProfile.NetworkInterfaces)
            //{
            //    Console.WriteLine("  networkInterface id: " + nic.Id);
            //}

            //Console.WriteLine("\nvmAgent");
            //Console.WriteLine("  vmAgentVersion" + vmResult.InstanceView.VmAgent.VmAgentVersion);
            //Console.WriteLine("    statuses");
            //foreach (InstanceViewStatus stat in vmResult.InstanceView.VmAgent.Statuses)
            //{
            //    Console.WriteLine("    code: " + stat.Code);
            //    Console.WriteLine("    level: " + stat.Level);
            //    Console.WriteLine("    displayStatus: " + stat.DisplayStatus);
            //    Console.WriteLine("    message: " + stat.Message);
            //    Console.WriteLine("    time: " + stat.Time);
            //}

            //Console.WriteLine("\ndisks");
            //foreach (DiskInstanceView idisk in vmResult.InstanceView.Disks)
            //{
            //    Console.WriteLine("  name: " + idisk.Name);
            //    Console.WriteLine("  statuses");
            //    foreach (InstanceViewStatus istat in idisk.Statuses)
            //    {
            //        Console.WriteLine("    code: " + istat.Code);
            //        Console.WriteLine("    level: " + istat.Level);
            //        Console.WriteLine("    displayStatus: " + istat.DisplayStatus);
            //        Console.WriteLine("    time: " + istat.Time);
            //    }
            //}

            //Console.WriteLine("\nVM general status");
            //Console.WriteLine("  provisioningStatus: " + vmResult.ProvisioningState);
            //Console.WriteLine("  id: " + vmResult.Id);
            //Console.WriteLine("  name: " + vmResult.Name);
            //Console.WriteLine("  type: " + vmResult.Type);
            //Console.WriteLine("  location: " + vmResult.Location);
            //Console.WriteLine("\nVM instance status");
            //foreach (InstanceViewStatus istat in vmResult.InstanceView.Statuses)
            //{
            //    Console.WriteLine("\n  code: " + istat.Code);
            //    Console.WriteLine("  level: " + istat.Level);
            //    Console.WriteLine("  displayStatus: " + istat.DisplayStatus);
            //}
            return vmResult.InstanceView.Statuses[1].DisplayStatus;
        }


        //остановка виртуальной машины
        public static async void StopVirtualMachineAsync(
            TokenCredentials credential,
            string groupName,
            string vmName,
            string subscriptionId)
        {
            var computeManagementClient = new ComputeManagementClient(credential)
            { SubscriptionId = subscriptionId };

            await computeManagementClient.VirtualMachines.DeallocateAsync(groupName, vmName);
            //await computeManagementClient.VirtualMachines.PowerOffAsync(groupName, vmName);
            //var image = new Image("UKSouth")
            //{
            //    StorageProfile = new ImageStorageProfile()
            //    {
            //        OsDisk = new ImageOSDisk()
            //        {
            //            OsType = OperatingSystemTypes.Windows,
            //            OsState = OperatingSystemStateTypes.Generalized,
            //            ManagedDisk = new Microsoft.Azure.Management.Compute.Models.SubResource("/subscriptions/a755ef57-8bdd-447e-bd18-9f89ae802903/resourceGroups/NEWGROUP1/providers/Microsoft.Compute/disks/SimpleWinVM_OsDisk_1_cfe9957945db428bb37a30b85447a429"),
            //        }
            //    }
            //};
            //await computeManagementClient.Images.CreateOrUpdateAsync(groupName, "imagechromenew", image);
            //await computeManagementClient.VirtualMachines.DeallocateAsync(groupName, vmName);
            //await computeManagementClient.VirtualMachines.CaptureAsync(groupName, vmName, new VirtualMachineCaptureParameters(vmName, groupName, true));
        }

        //запуск виртуальной машины
        public static async void StartVirtualMachineAsync(
            TokenCredentials credential,
            string groupName,
            string vmName,
            string subscriptionId)
        {
            var computeManagementClient = new ComputeManagementClient(credential)
            { SubscriptionId = subscriptionId };
            await computeManagementClient.VirtualMachines.StartAsync(groupName, vmName);
        }

        //Рестарт виртуальной машины
        //public static async void RestartVirtualMachineAsync(
        //    TokenCredentials credential,
        //    string groupName,
        //    string vmName,
        //    string subscriptionId)
        //{
        //    Console.WriteLine("Restarting the virtual machine...");
        //    var computeManagementClient = new ComputeManagementClient(credential)
        //    { SubscriptionId = subscriptionId };
        //    await computeManagementClient.VirtualMachines.RestartAsync(groupName, vmName);
        //}

        //Удаление виртуальной машины
        //public static async void DeleteVirtualMachineAsync(
        //    TokenCredentials credential,
        //    string groupName,
        //    string vmName,
        //    string subscriptionId)
        //{
        //    Console.WriteLine("Deleting the virtual machine...");
        //    var computeManagementClient = new ComputeManagementClient(credential)
        //    { SubscriptionId = subscriptionId };
        //    await computeManagementClient.VirtualMachines.DeleteAsync(groupName, vmName);
        //}

        private async void addVMButton_Click(object sender, EventArgs e)
        {
            var input = new InputResourceGroupName();
            input.Activate();
            input.ShowDialog(this);
            if (String.IsNullOrEmpty(input.GroupName)) return;
            addVMButton.Enabled = false;
            WaitForm waitform = new WaitForm();
            waitform.Message = "Подождите, идет создание ресурсов...";
            waitform.Start();
            string message = "";
            try
            {
                var rgResult = await CreateResourceGroupAsync(credential, input.GroupName, subscriptionId, location);

                if (rgResult.Properties.ProvisioningState == "Succeeded")
                {
                    var dpResult = await CreateTemplateDeploymentAsync(
                        credential,
                        input.GroupName,
                        deploymentName,
                        subscriptionId);
                    if (dpResult.Properties.ProvisioningState == "Succeeded")
                    {
                        message = "Ресурсы успешно созданы";
                        Resources.Add(new VMModel { ResourceGroupName = input.GroupName });
                    }
                    else
                    {
                        DeleteResourceGroupAsync(
                            credential,
                            input.GroupName,
                            subscriptionId);
                        message = "Ресурсы не удалось создать, попробуйте позже";
                    }
                }
                else
                    message = "Группу ресурсов не удалось создать";
            }
            catch
            {
                DeleteResourceGroupAsync(
                            credential,
                            input.GroupName,
                            subscriptionId);
                message = "Группу ресурсов не удалось создать";
            }
            finally
            {
                waitform.Stop();
                MessageBox.Show(message, "VMManager", MessageBoxButtons.OK);
                addVMButton.Enabled = true;
            }
        }

        private void deleteVMButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count > 0)
            {
                DialogResult res = MessageBox.Show("Действительно удалить виртуальную машину?", "Message CreateResourceGroupAsync", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    string selectedGroupName = dataGridView.SelectedCells[0].OwningRow.Cells[0].Value as string;
                    DeleteResourceGroupAsync(
                        credential,
                        selectedGroupName,
                        subscriptionId);
                    var item = Resources.FirstOrDefault(x => x.ResourceGroupName == selectedGroupName);
                    Resources.Remove(item);
                    //DeleteBlobsThread(selectedGroupName);
                }
            }
        }

        private void DeleteBlobsThread(string name)
        {
            //Thread threadCredentials = new Thread(UpdateCredentials);
            Thread thread = new Thread(DeleteBlobs);

            thread.IsBackground = /*threadCredentials.IsBackground =*/ true;

            //threadCredentials.Start();
            thread.Start(name);
        }

        private async void DeleteBlobs(object name)
        {
            var success = false;
            var selectedGroupName = name as String;
            while (!success)
            {
                try
                {
                    DeleteStorageVHD(selectedGroupName);
                    success = true;
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }
        }

        private void startVMButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                string selectedGroupName = dataGridView.SelectedRows[0].Cells[0].Value as string;
                StartVirtualMachineAsync(
                credential,
                selectedGroupName,
                VMName,
                subscriptionId);
            }
        }

        private void stopVMButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                DialogResult res = MessageBox.Show("Действительно остановить виртуальную машину?", "Message CreateResourceGroupAsync", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    string selectedGroupName = dataGridView.SelectedRows[0].Cells[0].Value as string;
                    StopVirtualMachineAsync(
                    credential,
                    selectedGroupName,
                    VMName,
                    subscriptionId);
                }
            }
        }
        private void SetupButtonsState()
        {
            if (Resources.Count == 0)
            {
                startVMButton.Enabled = false;
                stopVMButton.Enabled = false;
                deleteVMButton.Enabled = false;
                stopAllButton.Enabled = false;
                rdpButton.Enabled = false;
            }
            else if (dataGridView.SelectedRows.Count > 0)
            {
                if (Resources.Any(x => x.VMStatus == "VM running"))
                {
                    stopAllButton.Enabled = true;
                }
                else
                {
                    stopAllButton.Enabled = false;
                }
                deleteVMButton.Enabled = true;
                string selectedGroupName = dataGridView.SelectedRows[0].Cells[0].Value as string;
                var group = Resources.FirstOrDefault(x => x.ResourceGroupName == selectedGroupName);
                if (group != null)
                {
                    switch (group.VMStatus)
                    {
                        case "VM running":
                            stopVMButton.Enabled = true;
                            rdpButton.Enabled = true;
                            startVMButton.Enabled = false;
                            break;
                        case "VM stopped":
                            startVMButton.Enabled = true;
                            stopVMButton.Enabled = false;
                            rdpButton.Enabled = false;
                            break;
                        case "VM deallocated":
                            startVMButton.Enabled = true;
                            stopVMButton.Enabled = false;
                            rdpButton.Enabled = false;
                            break;
                        default:
                            stopVMButton.Enabled = false;
                            startVMButton.Enabled = false;
                            rdpButton.Enabled = false;
                            break;
                    }
                }
            }
        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //this.dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            SetupButtonsState();
        }

        private void stopAllButton_Click(object sender, EventArgs e)
        {
            if (Resources != null)
            {
                DialogResult res = MessageBox.Show("Действительно остановить все виртуальные машины?", "Message CreateResourceGroupAsync", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    foreach (var item in Resources)
                    {
                        StopVirtualMachineAsync(
                            credential,
                            item.ResourceGroupName,
                            VMName,
                            subscriptionId);
                    }
                }
            }
        }

        private void rdpButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                string selectedGroupName = dataGridView.SelectedRows[0].Cells[0].Value as string;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = String.Format("/C cmdkey /generic:\"{0}.uksouth.cloudapp.azure.com\" /user:\"{1}\" /pass:\"{2}\" & mstsc /v:{3}.uksouth.cloudapp.azure.com", selectedGroupName, StaticAuth.Username, StaticAuth.Password, selectedGroupName);
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            SetupButtonsState();
        }
    }
}
