using System;
using System.Collections.Generic;
using System.Text;

namespace SelfControl.Helpers
{
    class GalleryPageModel : BaseViewModel
    {
        ICommand _previewImageCommand = null;
        public static ObservableCollection<GalleryImage> _images = new ObservableCollection<GalleryImage>();
        ImageSource _previewImage = null;


        public GalleryPageModel()
        {
        }

        public ObservableCollection<GalleryImage> Images
        {
            get { return _images; }
        }

        public ImageSource PreviewImage
        {
            get { return _previewImage; }
            set
            {
                SetProperty(ref _previewImage, value);
            }
        }

        public ICommand PreviewImageCommand
        {
            get
            {
                return _previewImageCommand ?? new Command<Guid>((img) => {

                    var image = _images.Single(x => x.ImageId == img).OrgImage;

                    PreviewImage = ImageSource.FromStream(() => new MemoryStream(image));

                });
            }
        }
    }
}
