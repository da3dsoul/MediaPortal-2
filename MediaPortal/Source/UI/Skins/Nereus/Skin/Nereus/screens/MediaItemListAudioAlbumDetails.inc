<?xml version="1.0" encoding="utf-8"?>
<Grid
  xmlns="www.team-mediaportal.com/2008/mpf/directx"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:fanart="clr-namespace:MediaPortal.Extensions.UserServices.FanArtService.Client;assembly=FanArtService.Client"
  xmlns:media="clr-namespace:MediaPortal.UiComponents.Media.Models.AspectWrappers;assembly=Media"
  xmlns:aspect="clr-namespace:MediaPortal.Common.MediaManagement.DefaultItemAspects;assembly=MediaPortal.Common"
  Grid.Column="0" Grid.Row="0" x:Name="MediaItemDetails" Margin="0,200,20,-10"
  HorizontalAlignment="Stretch" VerticalAlignment="Stretch">

  <Grid.Resources>
    <fanart:FanArtImageSourceConverter x:Key="FanArtImageSourceConverter"/>
    <MultiAspectAttributeConverter x:Key="MultiAspectAttributeConverter"/>
  </Grid.Resources>

  <Grid.RowDefinitions>
    <RowDefinition Height="*"/>
    <RowDefinition Height="456"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
    <ColumnDefinition Width="456"/>
  </Grid.ColumnDefinitions>

  <Grid Grid.Row="1" Grid.Column="1" Height="456" Width="456" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
    <Grid.RowDefinitions>
      <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <Image Stretch="Fill" Margin="0,-190,0,0" Width="420" Height="420" HorizontalAlignment="Center" VerticalAlignment="Top">
      <Image.Source>
        <ImageSourceWrapper Delay="0.5" DelayInOut="true" VerticalTextureAlignment="Center"
                            Source="{Binding Source={StaticResource FanArtBackgroundModel}, Path=ImageSource, Converter={StaticResource FanArtImageSourceConverter}, ConverterParameter=DiscArt;420;420}"/>
      </Image.Source>
    </Image>
    <Image HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Stretch="Fill" Width="456" Height="456">
      <Image.Source>
        <ImageSourceWrapper Delay="0.5" DelayInOut="true" Transition="transitions\dissolve" TransitionDuration="0.5"
                            Source="Square_large.png"/>
      </Image.Source>
    </Image>
    <Image x:Name="AlbumCover" HorizontalAlignment="Center" VerticalAlignment="Center" Stretch="Fill" Margin="8" Width="440" Height="440">
      <Image.Source>
        <ImageSourceWrapper Delay="0.5" DelayInOut="true" Transition="transitions\dissolve" TransitionDuration="0.5"
                            Source="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"/>
      </Image.Source>
    </Image>
    <Image HorizontalAlignment="Center" VerticalAlignment="Center" Stretch="Uniform" Width="200" Height="200" 
           Source="audio.png" IsVisible="{Binding ElementName=AlbumCover, Path=!HasImage}"/>
  </Grid>

  <media:AudioAlbumAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"
                                 HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Margin="0,0,20,0" Grid.RowSpan="2">
    <media:AudioAlbumAspectWrapper.Template>
      <ControlTemplate>
        <Grid HorizontalAlignment="Stretch" VerticalAlignment="Stretch" DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
          <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
          </Grid.RowDefinitions>
          <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
          </Grid.ColumnDefinitions>

          <StackPanel Grid.Row="0" Orientation="Vertical" Margin="0,0,-500,0">

            <Label Content="{Binding Path=Album}" Wrap="True" Margin="0,0,0,0" Color="{ThemeResource TextColor}"
                   FontFamily="SegoeUIBold" FontSize="{ThemeResource NormalFontSize}" TextTrimming="WordEllipsis"
                   IsVisible="{Binding Path=Album,Converter={StaticResource ReferenceToBoolConverter}}"/>

            <Label Content="{Binding Path=Artists}" FontSize="{ThemeResource SmallFontSize}" Color="{ThemeResource TextColor}"
                   TextTrimming="WordEllipsis" Wrap="True" IsVisible="{Binding Path=Artists,Converter={StaticResource ReferenceToBoolConverter}}"/>

            <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" VerticalAlignment="Center" Margin="0,3,0,3"
                        IsVisible="{Binding Path=TotalRating,Converter={StaticResource ReferenceToBoolConverter}}">
              <StarRatingPanel x:Name="RatingReadOnly" IsReadOnly="True" StarReadOnlyTemplate="{StaticResource StarReadOnlyTemplate}"
                               Maximum="5" VerticalAlignment="Center">
                <StarRatingPanel.Value>
                  <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0}/2">
                    <Binding Path="TotalRating"/>
                  </MultiBinding>
                </StarRatingPanel.Value>
                <StarRatingPanel.LayoutTransform>
                  <TransformGroup>
                    <ScaleTransform ScaleX="1.6" ScaleY="1.5"/>
                  </TransformGroup>
                </StarRatingPanel.LayoutTransform>
              </StarRatingPanel>
              <Label x:Name="RatingCountLabel" Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center"
                     FontSize="{ThemeResource SmallerFontSize}" Margin="15,0,5,1"
                     IsVisible="{Binding Path=TotalRating,Converter={StaticResource ReferenceToBoolConverter}}">
                <Label.Content>
                  <Binding Path="TotalRating" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}({0:F1})"/>
                </Label.Content>
              </Label>
            </StackPanel>

            <media:MediaAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
              <media:MediaAspectWrapper.Template>
                <ControlTemplate>
                  <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                        IsVisible="{Binding ElementName=Year, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
                    <Image Source="HomeContentIcons\year.png" Height="27" Width="27" Stretch="Uniform" Margin="0,-1,5,1"
                           VerticalAlignment="Center" HorizontalAlignment="Left"/>
                    <Label x:Name="Year" FontSize="{ThemeResource SmallFontSize}" VerticalAlignment="Center" Margin="40,0,0,0"
                           Content="{Binding Path=RecordingTime, Converter={StaticResource DateFormatConverter}, ConverterParameter=yyyy}" Color="{ThemeResource TextColorForeground}"/>
                  </Grid>
                </ControlTemplate>
              </media:MediaAspectWrapper.Template>
            </media:MediaAspectWrapper>

            <Grid IsVisible="{Binding ElementName=Genre, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
              <Image Source="HomeContentIcons\audiogenre.png" Height="30" Width="30" Stretch="Uniform" Margin="0,0,5,0" 
                     VerticalAlignment="Center" HorizontalAlignment="Left"/>
              <Label x:Name="Genre" FontSize="{ThemeResource SmallFontSize}" TextTrimming="WordEllipsis" 
                     Color="{ThemeResource TextColorForeground}" Margin="40,0,0,0">
                <Label.Content>
                  <MultiBinding Converter="{StaticResource MultiAspectAttributeConverter}">
                    <Binding Source="{StaticResource FanArtBackgroundModel}" Path="MediaItem"/>
                    <Binding Source="{x:Static aspect:GenreAspect.ATTR_GENRE}" />
                  </MultiBinding>
                </Label.Content>
              </Label>
            </Grid>

            <Grid DataContext="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem}"
                  IsVisible="{Binding Path=Tracks,Converter={StaticResource ReferenceToBoolConverter}}">
              <Image Source="HomeContentIcons\track.png" Height="29" Width="28" Stretch="Uniform" Margin="0,0,5,0"
                     VerticalAlignment="Center" HorizontalAlignment="Left"/>
              <StackPanel Orientation="Horizontal" Margin="40,0,0,0">
                <Label x:Name="NumTracks" Content="{Binding Tracks}" VerticalAlignment="Center"
                       Color="{ThemeResource TextColorForeground}" FontSize="{ThemeResource SmallFontSize}"/>
                <Label Content=" " IsVisible="{Binding Path=Tracks,Converter={StaticResource ReferenceToBoolConverter}}" VerticalAlignment="Center"/>
                <Label x:Name="NumTrackLabel" Content="[Media.TrackItemsHeader]" Color="{ThemeResource TextColorForeground}" FontSize="{ThemeResource SmallFontSize}"
                       IsVisible="{Binding Path=Tracks,Converter={StaticResource ReferenceToBoolConverter}}" VerticalAlignment="Center"/>
              </StackPanel>
            </Grid>

            <media:UserDataWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
              <media:UserDataWrapper.Template>
                <ControlTemplate>
                  <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                        IsVisible="{Binding Path=PlayPercentage,Converter={StaticResource ReferenceToBoolConverter}}">
                    <Image VerticalAlignment="Center" Stretch="Fill" Width="95" Height="95" Margin="0,10,40,0" >
                      <Image.Source>
                        <Binding Path="PlayPercentage" Converter="{StaticResource RoundingFormatConverter}" ConverterParameter="{}5;CircularBar\{0}.png"/>
                      </Image.Source>
                    </Image>
                    <Label VerticalAlignment="Center" HorizontalAlignment="Center" FontSize="20" Color="White" FontFamily="SegoeUIBold"
                           Width="95" Height="95" Margin="0,0,40,0" HorizontalContentAlignment="Center" VerticalContentAlignment="Center">
                      <Label.Content>
                        <Binding Path="PlayPercentage" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}{0:F0}%"/>
                      </Label.Content>
                    </Label>
                  </Grid>
                </ControlTemplate>
              </media:UserDataWrapper.Template>
            </media:UserDataWrapper>

          </StackPanel>

          <Label x:Name="Description" Grid.Row="1" Margin="0,40,0,30" TextTrimming="WordEllipsis"
                 HorizontalAlignment="Stretch" Wrap="True" Opacity="0.6" Content="{Binding Path=Description}"
                 FontSize="{ThemeResource SmallFontSize}" Color="{ThemeResource TextColor}"/>
        </Grid>
      </ControlTemplate>
    </media:AudioAlbumAspectWrapper.Template>
  </media:AudioAlbumAspectWrapper>

</Grid>
