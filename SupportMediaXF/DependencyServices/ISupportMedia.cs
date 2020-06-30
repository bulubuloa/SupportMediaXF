using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;

namespace SupportMediaXF.DependencyServices
{
    public interface ISupportMedia
    {
        Task<List<SupportImageXF>> IF_OpenGallery(SyncPhotoOptions options);
        Task<SupportImageXF> IF_OpenCamera(SyncPhotoOptions options);
        Task<SupportImageXF> IF_SyncPhotoFromCloud(SupportImageXF imageSet, SyncPhotoOptions options);
        Task<SupportImageXF> IF_WriteStreamToFile(SupportImageXF imageSet, SyncPhotoOptions options);
    }
}