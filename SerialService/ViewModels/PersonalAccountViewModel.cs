namespace SerialService.ViewModels
{
    using System.Collections.Generic;

    public class PersonalAccountViewModel
    {
        public string ID { get; set; }
        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }
        public string CurrentUserName { get; set; }
        public string NewUserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string CurrentAvatarURL { get; set; }
        public string NewParole { get; set; }

        public IEnumerable<VideoMaterialPersonalAccountViewModel> LikedVideoMaterials { get; set; }
        public IEnumerable<VideoMaterialPersonalAccountViewModel> DislikedVideoMaterials { get; set; }
        public IEnumerable<VideoMaterialPersonalAccountViewModel> SubscribedVideoMaterials { get; set; }
    }
}