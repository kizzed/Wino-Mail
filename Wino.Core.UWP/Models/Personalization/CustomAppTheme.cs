﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Wino.Core.Domain.Enums;
using Wino.Core.Domain.Models.Personalization;
using Wino.Services;

namespace Wino.Core.UWP.Models.Personalization
{
    /// <summary>
    /// Custom themes that are generated by users.
    /// </summary>
    public class CustomAppTheme : AppThemeBase
    {
        public CustomAppTheme(CustomThemeMetadata metadata) : base(metadata.Name, metadata.Id)
        {
            AccentColor = metadata.AccentColorHex;
        }

        public override AppThemeType AppThemeType => AppThemeType.Custom;

        public override string GetBackgroundPreviewImagePath()
            => $"ms-appdata:///local/{ThemeService.CustomThemeFolderName}/{Id}_preview.jpg";

        public override async Task<string> GetThemeResourceDictionaryContentAsync()
        {
            var customAppThemeFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Wino.Core.UWP/AppThemes/Custom.xaml"));
            return await FileIO.ReadTextAsync(customAppThemeFile);
        }
    }
}
