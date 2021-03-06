using System;
using System.Drawing;
using GameServer;

namespace NpcScriptEngine  {
	public class NpcDialog {

		public void Initialize(GameClient Client) {
			NpcDialogBuilder.Avatar(Client, 1);
			NpcDialogBuilder.Text(Client, "Do you wish to be teleported to jail?");
			NpcDialogBuilder.Option(Client, 0x00, "Yes, please");
			NpcDialogBuilder.Finish(Client);				
		}
		public void Process(GameClient Client, byte OptionID, string Input) {
			switch(OptionID) {
				case 0:
					Client.Teleport(6000, 28, 70);
				break;
			}
		}
	}
}