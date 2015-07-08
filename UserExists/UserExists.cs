using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using TShockAPI;
using TerrariaApi.Server;

namespace UserExists
{
	[ApiVersion(1, 18)]
	public class UserExists : TerrariaPlugin
	{
		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}
		
		public override string Name
		{
			get { return "UserExists"; }
		}
		
		public override string Author
		{
			get { return "Simon311"; }
		}

		public override string Description
		{
			get { return "A command for checking if user exists"; }
		}

		public UserExists(Terraria.Main game)
			: base(game)
		{
			Order = 2;
		}

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command("userexists", UE, "userexists", "ue"));
			Commands.ChatCommands.Add(new Command("checkbanned", IfBanned, "banned"));
			if (!TShock.Config.AllowRegisterAnyUsername) ServerApi.Hooks.ServerChat.Register(this, OnChat, 2);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !TShock.Config.AllowRegisterAnyUsername) ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
			base.Dispose(disposing);
		}

		private void OnChat(ServerChatEventArgs e)
		{
			if (e.Handled) return;

			TSPlayer player = TShock.Players[e.Who];

			if (player == null)
			{
				e.Handled = true;
				return;
			}

			if (e.Text.StartsWith("/register") && !TShock.Config.AllowRegisterAnyUsername && !player.IsLoggedIn)
			{
				var user = TShock.Users.GetUserByName(player.Name);
				if (user != null) 
				{
					e.Handled = true;
					player.SendMessage("We're sorry, but this name is already taken.", Color.DeepPink);
					if (player.Group.HasPermission("selfname")) player.SendMessage("Please use /selfname [newname] to change your name.", Color.DeepPink);
				}
			}
		}


		private void UE(CommandArgs args)
		{
			if (args.Player == null ) return;

			string uname = String.Join(" ", args.Parameters);
			if (!string.IsNullOrWhiteSpace(uname))
			{
				var user = TShock.Users.GetUserByName(uname);
				if (user != null)
				{
					DateTime LastSeen = DateTime.Parse(user.LastAccessed);
					args.Player.SendMessage(string.Format("User {0} exists.", uname), Color.DeepPink);
					args.Player.SendMessage(string.Format("{0} was last seen {1}.", uname, LastSeen.ToShortDateString(), LastSeen.ToShortTimeString()), Color.DeepPink);
					if (args.Player.Group.HasPermission("viewgroup"))
					{
						List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(user.KnownIps);
						string ip = KnownIps[KnownIps.Count - 1];
						DateTime Registered = DateTime.Parse(user.Registered);

						args.Player.SendMessage(string.Format("{0}'s group is {1}.", uname, user.Group), Color.DeepPink);
						args.Player.SendMessage(string.Format("{0}'s last known IP is {1}.", uname, ip), Color.DeepPink);
						args.Player.SendMessage(string.Format("{0}'s register date is {1}.", uname, Registered.ToShortDateString()), Color.DeepPink);
					}
				}
				else
					args.Player.SendMessage(string.Format("User {0} does not exist.", uname), Color.DeepPink);
			}
			else args.Player.SendErrorMessage("Syntax: /userexists [name].");
		}

		private void IfBanned(CommandArgs args)
		{
			if (args.Player == null) return;

			string uname = String.Join(" ", args.Parameters);
			if (!string.IsNullOrWhiteSpace(uname))
			{
				var ban = TShock.Bans.GetBanByName(uname);
				if (ban != null)
				{
					args.Player.SendMessage(string.Format("{0} (IP: {1}) is banned for: \"{2}\"", uname, ban.IP, ban.Reason), Color.DeepPink);
					var add = ".";
					bool perma = String.IsNullOrWhiteSpace(ban.Expiration);
					if (!perma) add = " with expiration date: " + DateTime.Parse(ban.Expiration).ToUniversalTime();
					args.Player.SendMessage(string.Format("on {0} by user \"{1}\"{2}", DateTime.Parse(ban.Date).ToUniversalTime(), ban.BanningUser, add), Color.DeepPink);
					if (!perma)
					{
						TimeSpan Remain = DateTime.Parse(ban.Expiration).ToUniversalTime() - DateTime.Now.ToUniversalTime();
						if (Remain.TotalSeconds < 0) args.Player.SendMessage("The ban has already expired!", Color.DeepPink);
						else args.Player.SendMessage(string.Format("The ban will expire in {0}.", Remain.ToString()), Color.DeepPink);
					}
				}
				else
					args.Player.SendMessage(string.Format("{0} is not banned.", uname), Color.DeepPink);
			}
			else args.Player.SendMessage("Usage: /banned name", Color.Red);
		}
	}
}