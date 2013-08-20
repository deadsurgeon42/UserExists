using System;
using TShockAPI;
using Terraria;

namespace UserExists
{
    [APIVersion(1, 12)]
    public class UserExists : TerrariaPlugin
    {
        public override Version Version
        {
            get { return new Version("1.0.0.0"); }
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

        public UserExists(Main game)
            : base(game)
        {
            Order = 2;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("userexists", UE, "userexists", "ue"));
            Commands.ChatCommands.Add(new Command("checkbanned", IfBanned, "banned"));
        }

        private void UE(CommandArgs args)
        {
            if( args.Player != null )
            {
                string uname = String.Join(" ", args.Parameters);
                if (uname != null & uname != "")
                {
                    var user = TShock.Users.GetUserByName(uname);
                    if (user != null)
                    {
                        args.Player.SendMessage(string.Format("User {0} exists.", uname), Color.DeepPink);
                        if (args.Player.Group.HasPermission("viewgroup")) { args.Player.SendMessage(string.Format("{0}'s group is {1}.", uname, user.Group), Color.DeepPink); }
                    }
                    else
                    {
                        args.Player.SendMessage(string.Format("User {0} does not exist.", uname), Color.DeepPink);
                    }
                }
                else { args.Player.SendMessage("Usage: /userexists name", Color.Red); }
            }
        }
        private void IfBanned(CommandArgs args)
        {
            if (args.Player != null)
            {
                string uname = String.Join(" ", args.Parameters);
                if (uname != null & uname != "")
                {
                    var ban = TShock.Bans.GetBanByName(uname);
                    if (ban != null)
                    {
                        args.Player.SendMessage(string.Format("{0} (IP: {1}) is banned for: {2}", uname, ban.IP, ban.Reason), Color.DeepPink);
                    }
                    else
                    {
                        args.Player.SendMessage(string.Format("{0} is not banned.", uname), Color.DeepPink);
                    }
                }
                else { args.Player.SendMessage("Usage: /banned name", Color.Red); }
            }
        }
    }
}