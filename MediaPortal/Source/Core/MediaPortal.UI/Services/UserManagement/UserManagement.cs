#region Copyright (C) 2007-2017 Team MediaPortal

/*
    Copyright (C) 2007-2017 Team MediaPortal
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaPortal.Common;
using MediaPortal.Common.Services.ServerCommunication;
using MediaPortal.Common.Settings;
using MediaPortal.Common.UserManagement;
using MediaPortal.Common.UserProfileDataManagement;
using MediaPortal.UI.General;
using MediaPortal.UI.ServerCommunication;

namespace MediaPortal.UI.Services.UserManagement
{
  public class UserManagement : IUserManagement
  {
    public static UserProfile UNKNOWN_USER = new UserProfile(Guid.Empty, "Unknown");

    private UserProfile _currentUser = null;
    private bool _applyRestrictions = false;
    private ICollection<string> _restrictionGroups = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

    public bool IsValidUser
    {
      get { return CurrentUser != UNKNOWN_USER; }
    }

    public UserProfile CurrentUser
    {
      get { return _currentUser ?? (_currentUser = GetOrCreateDefaultUser().TryWait() ?? UNKNOWN_USER); }
      set
      {
        bool changed = _currentUser != value;
        _currentUser = value;
        if (changed)
        {
          UserMessaging.SendUserMessage(UserMessaging.MessageType.UserChanged);
          // Set new user name to allow overriding settings
          ServiceRegistration.Get<ISettingsManager>().ChangeUserContext(_currentUser?.ProfileId.ToString());
        }
      }
    }

    public IUserProfileDataManagement UserProfileDataManagement
    {
      get
      {
        UPnPClientControlPoint controlPoint = ServiceRegistration.Get<IServerConnectionManager>().ControlPoint;
        return controlPoint != null ? controlPoint.UserProfileDataManagementService : null;
      }
    }

    public void RegisterRestrictionGroup(string restrictionGroup)
    {
      if (!string.IsNullOrWhiteSpace(restrictionGroup))
        foreach (var group in restrictionGroup.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
        {
          _restrictionGroups.Add(group);
        }
    }

    public ICollection<string> RestrictionGroups
    {
      get { return _restrictionGroups; }
    }

    public bool CheckUserAccess(IUserRestriction restrictedElement)
    {
      if (!IsValidUser || !CurrentUser.EnableRestrictionGroups || string.IsNullOrEmpty(restrictedElement.RestrictionGroup))
        return true;

      foreach (var group in restrictedElement.RestrictionGroup.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
        if (CurrentUser.RestrictionGroups.Contains(group))
          return true;
      return false;
    }

    public bool ApplyUserRestriction
    {
      get { return _applyRestrictions; }
      set { _applyRestrictions = value; }
    }

    public async Task<UserProfile> GetOrCreateDefaultUser()
    {
      string profileName = SystemInformation.ComputerName;
      IUserProfileDataManagement updm = UserProfileDataManagement;
      if (updm == null)
        return null;

      var result = await updm.GetProfileByNameAsync(profileName);
      if (result.Success)
        return result.Result;

      Guid profileId = await updm.CreateProfileAsync(profileName);
      result = await updm.GetProfileAsync(profileId);
      if (result.Success)
        return result.Result;
      return null;
    }
  }
}
