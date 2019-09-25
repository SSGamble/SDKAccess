//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
#if !UNITY_EDITOR && UNITY_ANDROID
    using System;
    using UnityEngine;

    /// <summary>
    /// The reward video Ad.
    /// </summary>
    public sealed class RewardVideoAd : IDisposable
    {
        private readonly AndroidJavaObject ad;

        /// <summary>
        /// Initializes a new instance of the <see cref="RewardVideoAd"/> class.
        /// </summary>
        internal RewardVideoAd(AndroidJavaObject ad)
        {
            this.ad = ad;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <summary>
        /// Sets the interaction listener for this Ad.
        /// </summary>
        public void SetRewardAdInteractionListener(
            IRewardAdInteractionListener listener)
        {
            var androidListener = new RewardAdInteractionListener(listener);
            this.ad.Call("setRewardAdInteractionListener", androidListener);
        }

        /// <summary>
        /// Sets the download listener.
        /// </summary>
        public void SetDownloadListener(IAppDownloadListener listener)
        {
            var androidListener = new AppDownloadListener(listener);
            this.ad.Call("setDownloadListener", androidListener);
        }

        /// <summary>
        /// Gets the interaction type.
        /// </summary>
        public int GetInteractionType()
        {
            return this.ad.Call<int>("getInteractionType");
        }

        /// <summary>
        /// Show the reward video Ad.
        /// </summary>
        public void ShowRewardVideoAd()
        {
            var activity = SDK.GetActivity();
            var runnable = new AndroidJavaRunnable(
                () => this.ad.Call("showRewardVideoAd", activity));
            activity.Call("runOnUiThread", runnable);
        }

        /// <summary>
        /// Sets whether to show the download bar.
        /// </summary>
        public void SetShowDownLoadBar(bool show)
        {
            this.ad.Call("setShowDownLoadBar", show);
        }

#pragma warning disable SA1300
#pragma warning disable IDE1006

        private sealed class RewardAdInteractionListener : AndroidJavaProxy
        {
            private readonly IRewardAdInteractionListener listener;

            public RewardAdInteractionListener(
                IRewardAdInteractionListener listener)
                : base("com.bytedance.sdk.openadsdk.TTRewardVideoAd$RewardAdInteractionListener")
            {
                this.listener = listener;
            }

            public void onAdShow()
            {
                UnityDispatcher.PostTask(() => this.listener.OnAdShow());
            }

            public void onAdVideoBarClick()
            {
                UnityDispatcher.PostTask(
                    () => this.listener.OnAdVideoBarClick());
            }

            public void onAdClose()
            {
                UnityDispatcher.PostTask(() => this.listener.OnAdClose());
            }

            public void onVideoComplete()
            {
                UnityDispatcher.PostTask(() => this.listener.OnVideoComplete());
            }

            public void onVideoError()
            {
                UnityDispatcher.PostTask(() => this.listener.OnVideoError());
            }

            public void onRewardVerify(
                bool rewardVerify, int rewardAmount, string rewardName)
            {
                UnityDispatcher.PostTask(() => this.listener.OnRewardVerify(
                    rewardVerify, rewardAmount, rewardName));
            }
        }

#pragma warning restore SA1300
#pragma warning restore IDE1006
    }
#endif
}
