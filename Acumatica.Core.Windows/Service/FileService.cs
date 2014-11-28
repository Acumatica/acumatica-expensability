using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Acumatica.Core.Windows.Service
{
    public class FileService : IFileService
    {
        public async Task<IFile> PickSingleFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            var selectedFile = await picker.PickSingleFileAsync();
            if (selectedFile == null)
            {
                return null;
            }
            else
            {
                byte[] fileBytes = null;
                using (IRandomAccessStreamWithContentType stream = await selectedFile.OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                }

                return new Acumatica.Core.Service.File(selectedFile.Name, fileBytes);
            }
        }
    }
}
