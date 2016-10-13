#region Copyright (C) 2007-2015 Team MediaPortal

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

using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.MediaManagement.Helpers;
using MediaPortal.Common.Services.Settings;
using MediaPortal.Common.Settings;
using MediaPortal.Extensions.OnlineLibraries.Matchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MediaPortal.Extensions.OnlineLibraries
{
  /// <summary>
  /// <see cref="OnlineMatcherService"/> searches for metadata from online sources.
  /// </summary>
  public class OnlineMatcherService
  {
    private static List<IMusicMatcher> MUSIC_MATCHERS = new List<IMusicMatcher>();
    private static List<ISeriesMatcher> SERIES_MATCHERS = new List<ISeriesMatcher>();
    private static List<IMovieMatcher> MOVIE_MATCHERS = new List<IMovieMatcher>();
    private static SettingsChangeWatcher<OnlineLibrarySettings> SETTINGS_CHANGE_WATCHER = null;

    static OnlineMatcherService()
    {
      MUSIC_MATCHERS.Add(MusicTheAudioDbMatcher.Instance);
      MUSIC_MATCHERS.Add(CDFreeDbMatcher.Instance);
      MUSIC_MATCHERS.Add(MusicBrainzMatcher.Instance);
      MUSIC_MATCHERS.Add(MusicFanArtTvMatcher.Instance);

      MOVIE_MATCHERS.Add(MovieTheMovieDbMatcher.Instance);
      MOVIE_MATCHERS.Add(MovieOmDbMatcher.Instance);
      MOVIE_MATCHERS.Add(MovieFanArtTvMatcher.Instance);

      SERIES_MATCHERS.Add(SeriesTvDbMatcher.Instance);
      SERIES_MATCHERS.Add(SeriesTheMovieDbMatcher.Instance);
      SERIES_MATCHERS.Add(SeriesTvMazeMatcher.Instance);
      SERIES_MATCHERS.Add(SeriesOmDbMatcher.Instance);
      SERIES_MATCHERS.Add(SeriesFanArtTvMatcher.Instance);

      //Load settings
      LoadSettings();

      //Save settings
      SaveSettings();

      SETTINGS_CHANGE_WATCHER = new SettingsChangeWatcher<OnlineLibrarySettings>();
      SETTINGS_CHANGE_WATCHER.SettingsChanged += SettingsChanged;
    }

    private static void LoadSettings()
    {
      OnlineLibrarySettings settings = ServiceRegistration.Get<ISettingsManager>().Load<OnlineLibrarySettings>();
      foreach (MatcherSetting setting in settings.MusicMatchers)
      {
        IMusicMatcher matcher = MUSIC_MATCHERS.Find(m => m.Id.Equals(setting.Id, StringComparison.InvariantCultureIgnoreCase));
        if (matcher != null)
        {
          matcher.Primary = setting.Primary;
          matcher.Enabled = setting.Enabled;
        }
      }
      foreach (MatcherSetting setting in settings.MovieMatchers)
      {
        IMovieMatcher matcher = MOVIE_MATCHERS.Find(m => m.Id.Equals(setting.Id, StringComparison.InvariantCultureIgnoreCase));
        if (matcher != null)
        {
          matcher.Primary = setting.Primary;
          matcher.Enabled = setting.Enabled;
        }
      }
      foreach (MatcherSetting setting in settings.SeriesMatchers)
      {
        ISeriesMatcher matcher = SERIES_MATCHERS.Find(m => m.Id.Equals(setting.Id, StringComparison.InvariantCultureIgnoreCase));
        if (matcher != null)
        {
          matcher.Primary = setting.Primary;
          matcher.Enabled = setting.Enabled;
        }
      }
    }

    private static void SaveSettings()
    {
      OnlineLibrarySettings settings = ServiceRegistration.Get<ISettingsManager>().Load<OnlineLibrarySettings>();
      List<MatcherSetting> list = new List<MatcherSetting>();
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        MatcherSetting setting = new MatcherSetting();
        setting.Id = matcher.Id;
        setting.Enabled = matcher.Enabled;
        setting.Primary = matcher.Primary;
        list.Add(setting);
      }
      settings.MusicMatchers = list.ToArray();

      list.Clear();
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        MatcherSetting setting = new MatcherSetting();
        setting.Id = matcher.Id;
        setting.Enabled = matcher.Enabled;
        setting.Primary = matcher.Primary;
        list.Add(setting);
      }
      settings.MovieMatchers = list.ToArray();

      list.Clear();
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        MatcherSetting setting = new MatcherSetting();
        setting.Id = matcher.Id;
        setting.Enabled = matcher.Enabled;
        setting.Primary = matcher.Primary;
        list.Add(setting);
      }
      settings.SeriesMatchers = list.ToArray();

      ServiceRegistration.Get<ISettingsManager>().Save(settings);
    }

    private static void SettingsChanged(object sender, EventArgs e)
    {
      LoadSettings();
    }

    #region Audio

    public static bool FindAndUpdateTrack(TrackInfo trackInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.FindAndUpdateTrack(trackInfo, matcher.Primary ? false : forceQuickMode);
#if DEBUG
        Logger.Debug("FindAndUpdateTrack: Media item {0} processed by {1} ({2} ms)", trackInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool FindAndUpdateTrackPerson(TrackInfo trackInfo, PersonInfo personInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.FindAndUpdateTrackPerson(trackInfo, personInfo, forceQuickMode);
#if DEBUG
        Logger.Debug("FindAndUpdateTrackPerson: Media item {0} processed by {1} ({2} ms)", trackInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateAlbumPersons(AlbumInfo albumInfo, string occupation, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateAlbumPersons(albumInfo, occupation, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateAlbumPersons: Media item {0} processed by {1} ({2} ms)", albumInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateTrackPersons(TrackInfo trackInfo, string occupation, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateTrackPersons(trackInfo, occupation, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateTrackPersons: Media item {0} processed by {1} ({2} ms)", trackInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateAlbumCompanies(AlbumInfo albumInfo, string companyType, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateAlbumCompanies(albumInfo, companyType, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateAlbumCompanies: Media item {0} processed by {1} ({2} ms)", albumInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateAlbum(AlbumInfo albumInfo, bool updateTrackList, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateAlbum(albumInfo, updateTrackList, matcher.Primary ? false : forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateAlbum: Media item {0} processed by {1} ({2} ms)", albumInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }

      if (updateTrackList)
      {
        if (albumInfo.Tracks.Count == 0)
          return false;

        for (int i = 0; i < albumInfo.Tracks.Count; i++)
        {
          //TrackInfo trackInfo = albumInfo.Tracks[i];
          //foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
          //{
          //  matcher.FindAndUpdateTrack(trackInfo, forceQuickMode);
          //}
        }
      }
      return success;
    }

    public static bool DownloadAudioFanArt(Guid mediaItemId, BaseInfo mediaItemInfo)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
      {
        sw.Restart();
        success |= matcher.ScheduleFanArtDownload(mediaItemId, mediaItemInfo);
#if DEBUG
        Logger.Debug("DownloadAudioFanArt: Media item {0} processed by {1} ({2} ms)", mediaItemId.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static void StoreAudioPersonMatch(PersonInfo person)
    {
      if (person.Occupation == PersonAspect.OCCUPATION_ARTIST)
      {
        foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
        {
          matcher.StoreArtistMatch(person);
        }
      }
      else if (person.Occupation == PersonAspect.OCCUPATION_COMPOSER)
      {
        foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
        {
          matcher.StoreComposerMatch(person);
        }
      }
    }

    public static void StoreAudioCompanyMatch(CompanyInfo company)
    {
      if (company.Type == CompanyAspect.COMPANY_MUSIC_LABEL)
      {
        foreach (IMusicMatcher matcher in MUSIC_MATCHERS)
        {
          matcher.StoreMusicLabelMatch(company);
        }
      }
    }

    #endregion

    #region Movie

    public static bool FindAndUpdateMovie(MovieInfo movieInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.FindAndUpdateMovie(movieInfo, matcher.Primary ? false : forceQuickMode);
#if DEBUG
        Logger.Debug("FindAndUpdateMovie: Media item {0} processed by {1} ({2} ms)", movieInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdatePersons(MovieInfo movieInfo, string occupation, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdatePersons(movieInfo, occupation, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdatePersons: Media item {0} processed by {1} ({2} ms)", movieInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateCharacters(MovieInfo movieInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateCharacters(movieInfo, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateCharacters: Media item {0} processed by {1} ({2} ms)", movieInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateCollection(MovieCollectionInfo collectionInfo, bool updateMovieList, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateCollection(collectionInfo, updateMovieList, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateCollection: Media item {0} processed by {1} ({2} ms)", collectionInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }

      if (updateMovieList)
      {
        if (collectionInfo.Movies.Count == 0)
          return false;

        for (int i = 0; i < collectionInfo.Movies.Count; i++)
        {
          //MovieInfo movieInfo = collectionInfo.Movies[i];
          //foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
          //{
          //  success |= matcher.FindAndUpdateMovie(movieInfo, forceQuickMode);
          //}
        }
      }
      return success;
    }

    public static bool UpdateCompanies(MovieInfo movieInfo, string companyType, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateCompanies(movieInfo, companyType, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateCompanies: Media item {0} processed by {1} ({2} ms)", movieInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool DownloadMovieFanArt(Guid mediaItemId, BaseInfo mediaItemInfo)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        sw.Restart();
        success |= matcher.ScheduleFanArtDownload(mediaItemId, mediaItemInfo);
#if DEBUG
        Logger.Debug("DownloadMovieFanArt: Media item {0} processed by {1} ({2} ms)", mediaItemId.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static void StoreMoviePersonMatch(PersonInfo person)
    {
      if (person.Occupation == PersonAspect.OCCUPATION_ACTOR)
      {
        foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
        {
          matcher.StoreActorMatch(person);
        }
      }
      else if (person.Occupation == PersonAspect.OCCUPATION_DIRECTOR)
      {
        foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
        {
          matcher.StoreDirectorMatch(person);
        }
      }
      else if (person.Occupation == PersonAspect.OCCUPATION_WRITER)
      {
        foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
        {
          matcher.StoreWriterMatch(person);
        }
      }
    }

    public static void StoreMovieCharacterMatch(CharacterInfo character)
    {
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        matcher.StoreCharacterMatch(character);
      }
    }

    public static void StoreMovieCompanyMatch(CompanyInfo company)
    {
      foreach (IMovieMatcher matcher in MOVIE_MATCHERS)
      {
        matcher.StoreCompanyMatch(company);
      }
    }

    #endregion

    #region Series

    public static bool FindAndUpdateEpisode(EpisodeInfo episodeInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.FindAndUpdateEpisode(episodeInfo, matcher.Primary ? false : forceQuickMode);
#if DEBUG
        Logger.Debug("FindAndUpdateEpisode: Media item {0} processed by {1} ({2} ms)", episodeInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateEpisodePersons(EpisodeInfo episodeInfo, string occupation, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateEpisodePersons(episodeInfo, occupation, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateEpisodePersons: Media item {0} processed by {1} ({2} ms)", episodeInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateEpisodeCharacters(EpisodeInfo episodeInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateEpisodeCharacters(episodeInfo, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateEpisodeCharacters: Media item {0} processed by {1} ({2} ms)", episodeInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateSeason(SeasonInfo seasonInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateSeason(seasonInfo, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateSeason: Media item {0} processed by {1} ({2} ms)", seasonInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateSeries(SeriesInfo seriesInfo, bool updateEpisodeList, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateSeries(seriesInfo, updateEpisodeList, matcher.Primary ? false : forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateSeries: Media item {0} processed by {1} ({2} ms)", seriesInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }

      if (updateEpisodeList)
      {
        if (seriesInfo.Episodes.Count == 0)
          return false;

        for (int i = 0; i < seriesInfo.Episodes.Count; i++)
        {
          //Gives more detail to the missing episodes but will be very slow
          //EpisodeInfo episodeInfo = seriesInfo.Episodes[i];
          //foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
          //{
          //  success |= matcher.FindAndUpdateEpisode(episodeInfo, forceQuickMode);
          //}
        }
      }
      return success;
    }

    public static bool UpdateSeriesPersons(SeriesInfo seriesInfo, string occupation, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateSeriesPersons(seriesInfo, occupation, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateSeriesPersons: Media item {0} processed by {1} ({2} ms)", seriesInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateSeriesCharacters(SeriesInfo seriesInfo, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateSeriesCharacters(seriesInfo, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateSeriesCharacters: Media item {0} processed by {1} ({2} ms)", seriesInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool UpdateSeriesCompanies(SeriesInfo seriesInfo, string companyType, bool forceQuickMode)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.UpdateSeriesCompanies(seriesInfo, companyType, forceQuickMode);
#if DEBUG
        Logger.Debug("UpdateSeriesCompanies: Media item {0} processed by {1} ({2} ms)", seriesInfo.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static bool DownloadSeriesFanArt(Guid mediaItemId, BaseInfo mediaItemInfo)
    {
      Stopwatch sw = new Stopwatch();
      bool success = false;
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        sw.Restart();
        success |= matcher.ScheduleFanArtDownload(mediaItemId, mediaItemInfo);
#if DEBUG
        Logger.Debug("DownloadSeriesFanArt: Media item {0} processed by {1} ({2} ms)", mediaItemId.ToString(), matcher.Id, sw.ElapsedMilliseconds);
#endif
      }
      return success;
    }

    public static void StoreSeriesPersonMatch(PersonInfo person)
    {
      if (person.Occupation == PersonAspect.OCCUPATION_ACTOR)
      {
        foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
        {
          matcher.StoreActorMatch(person);
        }
      }
      else if (person.Occupation == PersonAspect.OCCUPATION_DIRECTOR)
      {
        foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
        {
          matcher.StoreDirectorMatch(person);
        }
      }
      else if (person.Occupation == PersonAspect.OCCUPATION_WRITER)
      {
        foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
        {
          matcher.StoreWriterMatch(person);
        }
      }
    }

    public static void StoreSeriesCharacterMatch(CharacterInfo character)
    {
      foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
      {
        matcher.StoreCharacterMatch(character);
      }
    }

    public static void StoreSeriesCompanyMatch(CompanyInfo company)
    {
      if (company.Type == CompanyAspect.COMPANY_PRODUCTION)
      {
        foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
        {
          matcher.StoreCompanyMatch(company);
        }
      }
      else if (company.Type == CompanyAspect.COMPANY_TV_NETWORK)
      {
        foreach (ISeriesMatcher matcher in SERIES_MATCHERS)
        {
          matcher.StoreTvNetworkMatch(company);
        }
      }
    }

    #endregion

    public static ILogger Logger
    {
      get
      {
        return ServiceRegistration.Get<ILogger>();
      }
    }
  }
}
