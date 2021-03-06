using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CheckInActivity : Activity
	{
		private ResWealCheckIn _config;

		private uint _lastCheckTime;

		private byte _curFillPriceIndex;

		public uint EntranceParam
		{
			get
			{
				if (this._config != null)
				{
					return this._config.dwEntranceParam;
				}
				return 0u;
			}
		}

		public override uint ID
		{
			get
			{
				return this._config.dwID;
			}
		}

		public override COM_WEAL_TYPE Type
		{
			get
			{
				return 0;
			}
		}

		public RES_WEAL_CHEKIN_TYPE CheckInType
		{
			get
			{
				return this._config.bCheckInType;
			}
		}

		public bool CanFillIn
		{
			get
			{
				return this._config.bCanFillIn == 1;
			}
		}

		public uint FillInPriceID
		{
			get
			{
				return this._config.dwFillInPriceID;
			}
		}

		public uint LastCheckTime
		{
			get
			{
				return this._lastCheckTime;
			}
		}

		public uint FillPrice
		{
			get
			{
				ResFillInPrice resFillInPrice = null;
				if (GameDataMgr.wealCheckFillDict.TryGetValue(this.FillInPriceID, ref resFillInPrice) && (int)this._curFillPriceIndex < resFillInPrice.Price.Length)
				{
					return resFillInPrice.Price[(int)this._curFillPriceIndex];
				}
				return 0u;
			}
		}

		public RES_SHOPBUY_COINTYPE FillPriceCoin
		{
			get
			{
				return 2;
			}
		}

		public CheckInActivity(ActivitySys mgr, ResWealCheckIn config) : base(mgr, config.stCommon)
		{
			this._config = config;
			for (ushort num = 0; num < this._config.wDays; num += 1)
			{
				CheckInPhase ap = new CheckInPhase(this, (uint)num, this._config.astReward[(int)num]);
				base.AddPhase(ap);
			}
			this._lastCheckTime = 0u;
			this._curFillPriceIndex = 0;
		}

		public override uint GetVipAddition(int vipFlagBit)
		{
			if (vipFlagBit == 1)
			{
				return (uint)this._config.wQQVIPExtraRatio;
			}
			if (vipFlagBit != 16)
			{
				return base.GetVipAddition(vipFlagBit);
			}
			return (uint)this._config.wQQSuperVIPExtraRatio;
		}

		public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
		{
			this._lastCheckTime = actvInfo.get_stCheckIn().dwLastCheckTime;
			this._curFillPriceIndex = actvInfo.get_stCheckIn().bFillPriceIndex;
			base.SetPhaseMarks(actvInfo.get_stCheckIn().ullRewardMask);
		}

		public override void SetPhaseMarked(uint phaseId)
		{
			base.SetPhaseMarked(phaseId);
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return;
			}
			if (phaseId + 1u < (uint)base.PhaseList.get_Count())
			{
				base.PhaseList.get_Item((int)(phaseId + 1u))._NotifyStateChanged();
			}
		}
	}
}
