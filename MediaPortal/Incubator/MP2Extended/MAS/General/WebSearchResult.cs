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

using MediaPortal.Plugins.MP2Extended.Common;
using System;

namespace MediaPortal.Plugins.MP2Extended.MAS.General
{
  [Serializable]
  public class WebSearchResult : WebObject
  {
    public WebSearchResult()
    {
    }

    public WebMediaType Type { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Score { get; set; }
    public WebDictionary<object> Details { get; set; }
  }
}
