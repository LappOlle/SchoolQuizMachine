using SchoolQuizMachine.Models.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SchoolQuizMachine.Models.Handlers
{
    public class FileHandler
    {
        private IReadOnlyList<IStorageFile> itemCollection;
        private StorageFolder externalDevices;
        private StorageFolder internalDevice;
        private StorageFolder hardDrive;
        private StorageFolder usbDrive;

        public event EventHandler DevicesIsInitialized;

        public FileHandler()
        {
            externalDevices = KnownFolders.RemovableDevices;
            internalDevice = KnownFolders.PicturesLibrary;
            InitDevices();
        }

        private async void InitDevices()
        {
            try
            {
                usbDrive = (await externalDevices.GetFoldersAsync()).FirstOrDefault();
                hardDrive = (await internalDevice.GetFoldersAsync()).FirstOrDefault();
                await InitializeItems();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message + " ERRORMESSAGE");
            }
        }

        public IReadOnlyCollection<IStorageFile> GetItems()
        {
            return itemCollection;
        }

        private async Task CopyNewFilesToHardDrive()
        {

            try
            {
                if (usbDrive != null && hardDrive != null)
                {
                    itemCollection = null;
                    await DeleteItemsInHardDrive();
                    IReadOnlyList<IStorageFile> items = await usbDrive.GetFilesAsync();
                    items.OrderBy(n => n.Name);
                    foreach (var item in items)
                    {
                        await item.CopyAsync(hardDrive, item.Name, NameCollisionOption.ReplaceExisting);
                    }
                    await LoadFilesFromHarddrive();
                }
            }
            catch
            {

            }

        }

        public async Task SaveHighScore(List<Person> persons)
        {
            StorageFile highScore = await hardDrive.CreateFileAsync("Highscore.json", CreationCollisionOption.ReplaceExisting);
            var stream = await highScore.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            using (var dataWriter = new DataWriter(outputStream))
            {
                var jsonString = JsonConvert.SerializeObject(persons);
                dataWriter.WriteString(jsonString);
                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();
            }
            stream.Dispose();
        }
        private async Task LoadFilesFromHarddrive()
        {
            try
            {
                itemCollection = await hardDrive.GetFilesAsync();
                itemCollection.OrderBy(n => n.Name);
                if (DevicesIsInitialized != null)
                {
                    DevicesIsInitialized.Invoke(this, EventArgs.Empty);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message + " ERORR 2");
            }

        }
        private async Task DeleteItemsInHardDrive()
        {
            await hardDrive.DeleteAsync();
        }

        private async Task InitializeItems()
        {
            if (usbDrive != null)
            {
                await CopyNewFilesToHardDrive();
            }
            else
            {
                await LoadFilesFromHarddrive();
            }
        }
    }
}
