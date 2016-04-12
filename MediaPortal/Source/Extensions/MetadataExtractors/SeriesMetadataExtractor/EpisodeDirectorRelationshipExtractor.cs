﻿#region Copyright (C) 2007-2015 Team MediaPortal

/*
    Copyright (C) 2007-2015 Team MediaPortal
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
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Extensions.OnlineLibraries;
using MediaPortal.Common.MediaManagement.Helpers;

namespace MediaPortal.Extensions.MetadataExtractors.SeriesMetadataExtractor
{
  class EpisodeDirectorRelationshipExtractor : IRelationshipRoleExtractor
  {
    private static readonly Guid[] ROLE_ASPECTS = { VideoAspect.ASPECT_ID, EpisodeAspect.ASPECT_ID };
    private static readonly Guid[] LINKED_ROLE_ASPECTS = { PersonAspect.ASPECT_ID };

    public Guid Role
    {
      get { return EpisodeAspect.ROLE_EPISODE; }
    }

    public Guid[] RoleAspects
    {
      get { return ROLE_ASPECTS; }
    }

    public Guid LinkedRole
    {
      get { return PersonAspect.ROLE_PERSON; }
    }

    public Guid[] LinkedRoleAspects
    {
      get { return LINKED_ROLE_ASPECTS; }
    }

    public bool TryExtractRelationships(IDictionary<Guid, IList<MediaItemAspect>> aspects, out ICollection<IDictionary<Guid, IList<MediaItemAspect>>> extractedLinkedAspects, bool forceQuickMode)
    {
      extractedLinkedAspects = null;

      SingleMediaItemAspect videoAspect;
      if (!MediaItemAspect.TryGetAspect(aspects, VideoAspect.Metadata, out videoAspect))
        return false;

      IEnumerable<string> directors = videoAspect.GetCollectionAttribute<string>(VideoAspect.ATTR_DIRECTORS);

      // Build the person MI

      List<PersonInfo> persons = new List<PersonInfo>();
      if (directors != null) foreach (string person in directors) persons.Add(new PersonInfo() { Name = person, Occupation = PersonOccupation.Director });

      EpisodeInfo episodeInfo;
      if (!SeriesRelationshipExtractor.GetBaseInfo(aspects, out episodeInfo))
        return false;

      SeriesTheMovieDbMatcher.Instance.UpdateEpisodePersons(episodeInfo, persons, PersonOccupation.Director);
      SeriesTvMazeMatcher.Instance.UpdateEpisodePersons(episodeInfo, persons, PersonOccupation.Director);
      SeriesTvDbMatcher.Instance.UpdateEpisodePersons(episodeInfo, persons, PersonOccupation.Director);

      if (persons.Count == 0)
        return false;

      extractedLinkedAspects = new List<IDictionary<Guid, IList<MediaItemAspect>>>();

      foreach (PersonInfo person in persons)
      {
        IDictionary<Guid, IList<MediaItemAspect>> personAspects = new Dictionary<Guid, IList<MediaItemAspect>>();
        extractedLinkedAspects.Add(personAspects);
        person.SetMetadata(personAspects);
      }
      return true;
    }

    public bool TryMatch(IDictionary<Guid, IList<MediaItemAspect>> linkedAspects, IDictionary<Guid, IList<MediaItemAspect>> existingAspects)
    {
      if (!existingAspects.ContainsKey(PersonAspect.ASPECT_ID))
        return false;

      int linkedOccupation;
      if (!MediaItemAspect.TryGetAttribute(linkedAspects, PersonAspect.ATTR_OCCUPATION, out linkedOccupation))
        return false;

      int existingOccupation;
      if (!MediaItemAspect.TryGetAttribute(existingAspects, PersonAspect.ATTR_OCCUPATION, out existingOccupation))
        return false;

      return linkedOccupation == existingOccupation;
    }

    public bool TryGetRelationshipIndex(IDictionary<Guid, IList<MediaItemAspect>> aspects, out int index)
    {
      return MediaItemAspect.TryGetAttribute(aspects, VideoAspect.ATTR_DIRECTORS, out index);
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}