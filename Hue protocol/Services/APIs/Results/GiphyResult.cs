using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Newtonsoft.Json.Converters;
using HueProtocol.models;

namespace HueProtocol.Services.APIs.Results
{
    public class GiphyResult
    {
        public Data Data { get; set; }
        public Meta Meta { get; set; }
    }

    public partial class Data
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public Uri Url { get; set; }
        public string Slug { get; set; }
        public Uri BitlyGifUrl { get; set; }
        public Uri BitlyUrl { get; set; }
        public Uri EmbedUrl { get; set; }
        public string Username { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Rating { get; set; }
        public string ContentUrl { get; set; }
        public string SourceTld { get; set; }
        public string SourcePostUrl { get; set; }
        public long IsSticker { get; set; }
        public DateTimeOffset ImportDatetime { get; set; }
        public string TrendingDatetime { get; set; }
        public Images Images { get; set; }
        public User User { get; set; }
        public Uri ImageOriginalUrl { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri ImageMp4Url { get; set; }
        public long ImageFrames { get; set; }
        public long ImageWidth { get; set; }
        public long ImageHeight { get; set; }
        public Uri FixedHeightDownsampledUrl { get; set; }
        public long FixedHeightDownsampledWidth { get; set; }
        public long FixedHeightDownsampledHeight { get; set; }
        public Uri FixedWidthDownsampledUrl { get; set; }
        public long FixedWidthDownsampledWidth { get; set; }
        public long FixedWidthDownsampledHeight { get; set; }
        public Uri FixedHeightSmallUrl { get; set; }
        public Uri FixedHeightSmallStillUrl { get; set; }
        public long FixedHeightSmallWidth { get; set; }
        public long FixedHeightSmallHeight { get; set; }
        public Uri FixedWidthSmallUrl { get; set; }
        public Uri FixedWidthSmallStillUrl { get; set; }
        public long FixedWidthSmallWidth { get; set; }
        public long FixedWidthSmallHeight { get; set; }
        public string Caption { get; set; }
    }

    public partial class Images
    {
        public The480_WStill DownsizedLarge { get; set; }
        public The480_WStill FixedHeightSmallStill { get; set; }
        public FixedHeight Original { get; set; }
        public FixedHeight FixedHeightDownsampled { get; set; }
        public The480_WStill DownsizedStill { get; set; }
        public The480_WStill FixedHeightStill { get; set; }
        public The480_WStill DownsizedMedium { get; set; }
        public The480_WStill Downsized { get; set; }
        public The480_WStill PreviewWebp { get; set; }
        public DownsizedSmall OriginalMp4 { get; set; }
        public FixedHeight FixedHeightSmall { get; set; }
        public FixedHeight FixedHeight { get; set; }
        public DownsizedSmall DownsizedSmall { get; set; }
        public DownsizedSmall Preview { get; set; }
        public FixedHeight FixedWidthDownsampled { get; set; }
        public The480_WStill FixedWidthSmallStill { get; set; }
        public FixedHeight FixedWidthSmall { get; set; }
        public The480_WStill OriginalStill { get; set; }
        public The480_WStill FixedWidthStill { get; set; }
        public Looping Looping { get; set; }
        public FixedHeight FixedWidth { get; set; }
        public The480_WStill PreviewGif { get; set; }
        public The480_WStill The480WStill { get; set; }
    }

    public partial class The480_WStill
    {
        public Uri Url { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public long? Size { get; set; }
    }

    public partial class DownsizedSmall
    {
        public long Height { get; set; }
        public Uri Mp4 { get; set; }
        public long Mp4Size { get; set; }
        public long Width { get; set; }
    }

    public partial class FixedHeight
    {
        public long Height { get; set; }
        public Uri Mp4 { get; set; }
        public long? Mp4Size { get; set; }
        public long Size { get; set; }
        public Uri Url { get; set; }
        public Uri Webp { get; set; }
        public long WebpSize { get; set; }
        public long Width { get; set; }
        public long? Frames { get; set; }
        public string Hash { get; set; }
    }

    public partial class Looping
    {
        public Uri Mp4 { get; set; }
        public long Mp4Size { get; set; }
    }

    public partial class User
    {
        public Uri AvatarUrl { get; set; }
        public string BannerImage { get; set; }
        public string BannerUrl { get; set; }
        public Uri ProfileUrl { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public bool IsVerified { get; set; }
    }

    public partial class Meta
    {
        public long Status { get; set; }
        public string Msg { get; set; }
        public string ResponseId { get; set; }
    }
}