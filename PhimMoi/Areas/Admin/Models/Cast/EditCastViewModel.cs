﻿using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Admin.Models.Cast
{
    public class EditCastViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên.")]
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? About { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? AvatarFile { get; set; }

        [FileExtensions(Extensions = ".png,.jpg,.jpeg")]
        public string? AvatarFileName
        {
            get
            {
                if(AvatarFile != null)
                {
                    return AvatarFile.Name;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}