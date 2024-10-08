﻿using System;
using System.Collections.Generic;
using System.Text;
using ActivityManager.ViewModels.Activities;
using ActivityManager.ViewModels.Statistics;
using PCloudClient;
using VCore.Standard.Factories.ViewModels;
using VCore.WPF.ViewModels;
using VCore.WPF.ViewModels.Navigation;

namespace ActivityManager.ViewModels
{

  public class ActivityManagerMainWindowViewModel : BaseMainWindowViewModel
  {
    private readonly IViewModelsFactory viewModelsFactory;
    private readonly IPCloudService pCloudService;

    public ActivityManagerMainWindowViewModel(IViewModelsFactory viewModelsFactory, IPCloudService pCloudService) : base(viewModelsFactory)
    {
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.pCloudService = pCloudService ?? throw new ArgumentNullException(nameof(pCloudService));
    }

    public override string Title => "Manažer aktivít";

    public NavigationViewModel NavigationViewModel { get; set; } = new NavigationViewModel();

    public override void Initialize()
    {
      base.Initialize();

      var activities = viewModelsFactory.Create<ActivitiesViewModel>();
      var statistics = viewModelsFactory.Create<StatisticsViewModel>();

      var activitiesNav = new NavigationItem(activities);
      activitiesNav.IconPathData = "M142.1 316.9L122.2 363.1C121 366.1 118.1 368 115 368H40c-13.25 0-24 10.75-24 24S26.75 416 40 416h75c22.38 0 42.62-13.25 51.5-33.1L180 350.5l-9.5-5.75C158.6 337.5 149.2 327.9 142.1 316.9zM288 96c26.5 0 48-21.5 48-48s-21.5-48-48-48s-48 21.5-48 48S261.5 96 288 96zM408 240.5l-48.38-.125c-3.5 0-6.625-2.25-7.625-5.625l-13.88-46c-9.25-30.13-34-53.25-64.88-60.13L195 110.9C188.5 109.4 181.1 108.7 175.5 108.7c-19.27 0-38.11 6.396-53.72 18.46L73.38 164.5C67.27 169.2 64.08 176.2 64.08 183.4c0 12.45 9.888 24.04 23.98 24.04c5.158 0 10.35-1.649 14.69-5.047l48.37-37.25c7.096-5.509 15.73-8.367 24.48-8.367c2.967 0 5.949 .3281 8.891 .9923L199.4 161L164 248.4C161.2 255.3 159.9 262.4 159.9 269.4c0 19.12 9.79 37.46 27.07 47.98l83.75 50.5c2.476 1.486 3.932 4.149 3.932 6.933c0 .7289-.0999 1.466-.3074 2.192L241 481.4c-.6248 2.198-.9229 4.414-.9229 6.596C240.1 495.9 246.1 512 264.1 512c10.5 0 20.12-6.875 23.12-17.38l33.25-104.5c1.473-5.103 2.182-10.28 2.182-15.41c0-19.22-9.983-37.61-27.06-47.97L243.8 295.5l42-104.7c2.75 3.625 5 7.5 6.375 12l14 46c7.125 23.62 28.63 39.62 53.38 39.62l48.38 .25c13.25 0 24.12-10.75 24.12-24C432 251.4 421.4 240.6 408 240.5z";

      var statisticsNav = new NavigationItem(statistics);
      statisticsNav.IconPathData = "M488 432H48V56C48 42.74 37.25 32 24 32S0 42.74 0 56V448c0 17.6 14.4 32 32 32h456c13.25 0 24-10.75 24-24C512 442.7 501.3 432 488 432zM168 352C181.3 352 192 341.3 192 328v-48C192 266.8 181.3 256 168 256S144 266.8 144 280v48C144 341.3 154.8 352 168 352zM264 352c13.25 0 24-10.75 24-24v-176C288 138.8 277.3 128 264 128S240 138.8 240 152v176C240 341.3 250.8 352 264 352zM360 352c13.25 0 24-10.75 24-24v-112C384 202.8 373.3 192 360 192S336 202.8 336 216v112C336 341.3 346.8 352 360 352zM456 352c13.25 0 24-10.75 24-24v-208C480 106.8 469.3 96 456 96S432 106.8 432 120v208C432 341.3 442.8 352 456 352z";

      NavigationViewModel.Items.Add(activitiesNav);
      NavigationViewModel.Items.Add(statisticsNav);

      activities.IsActive = true;

      //pCloudService.SaveLoginInfo("pecho4@gmail.com", "roman564123a");
      //pCloudService.CreateUploadLink(folderId, "ActivityManager");


    }
  }
}
