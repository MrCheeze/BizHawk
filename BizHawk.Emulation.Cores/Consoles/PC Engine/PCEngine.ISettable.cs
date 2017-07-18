﻿using System.ComponentModel;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.PCEngine
{
	public sealed partial class PCEngine : ISettable<PCEngine.PCESettings, PCEngine.PCESyncSettings>
	{
		public PCESettings GetSettings()
		{
			return Settings.Clone();
		}

		public PCESyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public bool PutSettings(PCESettings o)
		{
			bool ret;
			if (o.ArcadeCardRewindHack != Settings.ArcadeCardRewindHack
				|| o.EqualizeVolume != Settings.EqualizeVolume)
			{
				ret = true;
			}
			else
			{
				ret = false;
			}

			Settings = o;
			return ret;
		}

		public bool PutSyncSettings(PCESyncSettings o)
		{
			bool ret = PCESyncSettings.NeedsReboot(o, _syncSettings);
			_syncSettings = o;
			// SetControllerButtons(); // not safe to change the controller during emulation, so instead make it a reboot event
			return ret;
		}

		internal PCESettings Settings;
		private PCESyncSettings _syncSettings;

		public class PCESettings
		{
			public bool ShowBG1 = true;
			public bool ShowOBJ1 = true;
			public bool ShowBG2 = true;
			public bool ShowOBJ2 = true;

			// these three require core reboot to use
			public bool SpriteLimit = false;
			public bool EqualizeVolume = false;
			public bool ArcadeCardRewindHack = true;

			public PCESettings Clone()
			{
				return (PCESettings)MemberwiseClone();
			}
		}

		public class PCESyncSettings
		{
			[DefaultValue(PceControllerType.GamePad)]
			[DisplayName("Port 1 Device")]
			[Description("The type of controller plugged into the first controller port")]
			[TypeConverter(typeof(DescribableEnumConverter))]
			public PceControllerType Port1 { get; set; } = PceControllerType.GamePad;

			[DefaultValue(PceControllerType.Unplugged)]
			[DisplayName("Port 2 Device")]
			[Description("The type of controller plugged into the second controller port")]
			[TypeConverter(typeof(DescribableEnumConverter))]
			public PceControllerType Port2 { get; set; } = PceControllerType.Unplugged;

			[DefaultValue(PceControllerType.Unplugged)]
			[DisplayName("Port 3 Device")]
			[Description("The type of controller plugged into the third controller port")]
			[TypeConverter(typeof(DescribableEnumConverter))]
			public PceControllerType Port3 { get; set; } = PceControllerType.Unplugged;

			[DefaultValue(PceControllerType.Unplugged)]
			[DisplayName("Port 4 Device")]
			[Description("The type of controller plugged into the fourth controller port")]
			[TypeConverter(typeof(DescribableEnumConverter))]
			public PceControllerType Port4 { get; set; } = PceControllerType.Unplugged;

			[DefaultValue(PceControllerType.Unplugged)]
			[DisplayName("Port 5 Device")]
			[Description("The type of controller plugged into the fifth controller port")]
			[TypeConverter(typeof(DescribableEnumConverter))]
			public PceControllerType Port5 { get; set; } = PceControllerType.Unplugged;

			public ControllerSetting[] Controllers =
			{
				new ControllerSetting { IsConnected = true },
				new ControllerSetting { IsConnected = false },
				new ControllerSetting { IsConnected = false },
				new ControllerSetting { IsConnected = false },
				new ControllerSetting { IsConnected = false }
			};

			public PCESyncSettings Clone()
			{
				var ret = new PCESyncSettings();
				for (int i = 0; i < Controllers.Length; i++)
				{
					ret.Controllers[i].IsConnected = Controllers[i].IsConnected;
				}

				return ret;
			}

			public class ControllerSetting
			{
				public bool IsConnected { get; set; }
			}

			public static bool NeedsReboot(PCESyncSettings x, PCESyncSettings y)
			{
				for (int i = 0; i < x.Controllers.Length; i++)
				{
					if (x.Controllers[i].IsConnected != y.Controllers[i].IsConnected)
					{
						return true;
					}
				}

				return false;
			}
		}
	}
}
