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
        }

        private void UE(CommandArgs args)
        {
            if( args.Player != null )
            {
                string uname = String.Join(" ", args.Parameters);
                if (uname != null & uname != "")
                {
                    if (TShock.Users.GetUserByName(uname) != null)
                    {
                        args.Player.SendMessage(string.Format("User {0} exists.", uname), Color.DeepPink);
                    }
                    else
                    {
                        args.Player.SendMessage(string.Format("User {0} does not exist.", uname), Color.DeepPink);
                    }
                }
                else { args.Player.SendMessage("Usage: /userexists name", Color.Red); }
            }
        }
    }
}