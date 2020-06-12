﻿#region Copyright (C) 2007-2018 Team MediaPortal

/*
    Copyright (C) 2007-2018 Team MediaPortal
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

using System.Runtime.Serialization;

namespace Emulators.Common.RawG.Data
{
  [DataContract]
  public class Creator : NamedItemCountImage
  {
    [DataMember(Name = "image")]
    public string IamgeUrl { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "rating")]
    public double? Rating { get; set; }

    [DataMember(Name = "rating_top")]
    public double? RatingTop { get; set; }

    [DataMember(Name = "updated")]
    public string Updated { get; set; }

    [DataMember(Name = "reviews_count")]
    public int? ReviewsCount { get; set; }
  }
}
