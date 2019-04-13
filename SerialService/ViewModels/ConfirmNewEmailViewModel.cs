namespace SerialService.ViewModels
{
	public class ConfirmNewEmailViewModel
	{
		public string UserID { get; set; }
		public string OldEmail { get; set; }
		public string NewEmail { get; set; }
		public string Code { get; set; }
		public string ResultMessage { get; set; }
	}
}