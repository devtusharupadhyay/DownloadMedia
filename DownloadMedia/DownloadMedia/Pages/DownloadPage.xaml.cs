using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DownloadMedia.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPage : ContentPage
    {
        public DownloadPage()
        {
            InitializeComponent();
        }

        private void StartDownload_Clicked(object sender, EventArgs e)
        {
            try
            {
                 DownLoadFile();
                //var task = Task.Run((Func<Task>)Run);
                //task.Wait();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }


        }

        async Task DownLoadFile()
        {
            string accessToken = "6xXCAAPwwBMAAAAAAAAAAVdWv90jkDLxNWK-aJwnuG3MB2hPb48LM4alQ3dX4cGW";
            string apiKey = "j7wgim5upso0rwf";
            string appSecret = "n2l24yigt9ah6pt";
            try
            {
                DropboxClient dbx = new DropboxClient(accessToken,
                    new DropboxClientConfig() { HttpClient = new HttpClient(new HttpClientHandler()) });

                Dropbox.Api.Users.FullAccount full = await dbx.Users.GetCurrentAccountAsync();

                Dropbox.Api.Files.ListFolderResult list = await dbx.Files.ListFolderAsync(string.Empty);

                var videoFile = list.Entries.Where(i => i.IsFile
                && i.Name.Contains("sample-mp4-file")
                ).LastOrDefault();

                string folder = "";
                string file = videoFile.Name;
                DisplayMessage.Text = file; //to display name on screen
                var response = await dbx.Files.DownloadAsync(folder + "/" + file);


                //local folder
                var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                if (!Directory.Exists(systemPath))
                {
                    System.IO.Directory.CreateDirectory(systemPath);
                }
                string path = Path.Combine(systemPath, file);

                //progressBar
                ulong fileSize = response.Response.Size;
                const int bufferSize = 1024 * 1024;
                byte[] buffer = new byte[bufferSize];
                Stream stream = await response.GetContentAsStreamAsync();
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                int length = stream.Read(buffer, 0, bufferSize);
                int maxLength = length;
                while (length > 0)
                {
                    fileStream.Write(buffer, 0, length);
                    ulong percentage = 100 * (ulong)fileStream.Length / fileSize;
                    var progressbarValue = (double)fileStream.Length / fileSize;
                    // Update progress bar with the percentage.
                    // progressBar.Value = (int)percentage
                    Debug.WriteLine("======================percentage=====================");
                    Debug.WriteLine(percentage);
                    Debug.WriteLine(progressbarValue);

                    length = stream.Read(buffer, 0, bufferSize);
                    await DownloadProgress.ProgressTo(progressbarValue, 500, Easing.Linear);
                }

                //var save = response.GetContentAsByteArrayAsync();
                //save.Wait();
                //var result = save.Result;



                //System.IO.File.WriteAllText(path, Convert.ToBase64String(result));

                Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);

            }
            catch (BadInputException iex)
            {
                await DisplayAlert("Error", iex.Message, "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task<PermissionStatus> CheckAndRequestStoragePermission()
        {

            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestStorageReadPermission()
        {

            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageRead>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.StorageRead>();

            return status;
        }

        async Task Run()
        {
            string accessToken = "6xXCAAPwwBMAAAAAAAAAAVdWv90jkDLxNWK-aJwnuG3MB2hPb48LM4alQ3dX4cGW";
            string apiKey = "j7wgim5upso0rwf";
            string appSecret = "n2l24yigt9ah6pt";
            try
            {
                using (var dbx = new DropboxClient(accessToken,
                    new DropboxClientConfig() { HttpClient = new HttpClient(new HttpClientHandler()) }))
                {
                    var full = await dbx.Users.GetCurrentAccountAsync();

                    var list = await dbx.Files.ListFolderAsync(string.Empty);


                    foreach (var item in list.Entries.Where(i => i.IsFile))
                    {
                        Debug.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
                    }

                    var videoFile = list.Entries.Where(i => i.IsFile
                    && i.Name.Contains(".xlsx")
                    ).LastOrDefault();

                    string folder = "";
                    string file = videoFile.Name;
                    var response = await dbx.Files.DownloadAsync(folder + "/" + file);


                    //local folder
                    var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    if (!Directory.Exists(systemPath))
                    {
                        System.IO.Directory.CreateDirectory(systemPath);
                    }
                    string path = Path.Combine(systemPath, file);

                    //progressBar
                    ulong fileSize = response.Response.Size;
                    const int bufferSize = 1024 * 1024;
                    byte[] buffer = new byte[bufferSize];
                    using (Stream stream = await response.GetContentAsStreamAsync())
                    {
                        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            int length = stream.Read(buffer, 0, bufferSize);
                            int maxLength = length;
                            while (length > 0)
                            {
                                fileStream.Write(buffer, 0, length);
                                ulong percentage = 100 * (ulong)fileStream.Length / fileSize;
                                // Update progress bar with the percentage.
                                // progressBar.Value = (int)percentage
                                Debug.WriteLine("======================percentage=====================");
                                Debug.WriteLine(percentage);

                                length = stream.Read(buffer, 0, bufferSize);
                                DownloadProgress.ProgressTo(length / maxLength, 500, Easing.Linear);

                            }
                        }
                    }


                    //var save = response.GetContentAsByteArrayAsync();
                    //save.Wait();
                    //var result = save.Result;



                    //System.IO.File.WriteAllText(path, Convert.ToBase64String(result));

                    Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
                }
            }
            catch (BadInputException iex)
            {
                await DisplayAlert("Error", iex.Message, "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }



        async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Debug.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Debug.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }

        async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                Debug.WriteLine(await response.GetContentAsStringAsync());
            }
        }

        private void TakePermission_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _ = CheckAndRequestStoragePermission();
                _ = CheckAndRequestStorageReadPermission();
            });
        }

        private void ReadFile_Clicked(object sender, EventArgs e)
        {
            try
            {
                var path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "/Book.xlsx";
                var readText = System.IO.File.ReadAllText(path);
            }
            catch (ArgumentNullException arguNullEx)
            { }
            catch (ArgumentException arguEx)
            { }
            catch (PathTooLongException pathEx) { }
            catch (DirectoryNotFoundException dirEx) { }
            catch (FileNotFoundException fileEx) { }
            catch (IOException ioEx) { }
            catch (UnauthorizedAccessException unAuthEx) { }
            catch (NotSupportedException ex) { }
            catch (System.Security.SecurityException securityEx) { }
            catch (Exception)
            {
                //throw;
            }


            //var fileByteArrray = Convert.FromBase64String(readText);
        }
    }
}