#region Copyright (C) 2007-2020 Team MediaPortal

/*
    Copyright (C) 2007-2020 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

namespace MediaPortal.Plugins.MP2Extended.MAS.OnlineVideos
{
  public class WebOnlineVideosVideo
  {
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AirDate { get; set; }
    public string Length { get; set; }
    public string StartTime { get; set; }
    public string SubtitleText { get; set; }
    public string SubtitleUrl { get; set; }
    public string VideoUrl { get; set; }
    public string ThumbUrl { get; set; }

    public override string ToString()
    {
      return Title;
    }
  }
}
