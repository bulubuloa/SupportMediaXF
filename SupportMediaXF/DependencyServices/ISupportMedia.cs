using System;
using System.Threading.Tasks;
using SupportMediaXF.Interfaces;
using SupportMediaXF.Models;

namespace SupportMediaXF.DependencyServices
{
    public interface ISupportMedia
    {
        void IF_OpenGallery(ISupportMediaResultListener supportMediaResultListener, SyncPhotoOptions options, int CodeRequest);
        void IF_OpenCamera(ISupportMediaResultListener supportMediaResultListener, SyncPhotoOptions options, int CodeRequest);
        Task<SupportImageXF> IF_SyncPhotoFromCloud(ISupportMediaResultListener supportMediaResultListener, SupportImageXF imageSet, SyncPhotoOptions options);
    }
}
