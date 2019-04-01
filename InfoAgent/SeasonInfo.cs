namespace InfoAgent
{
    public class SeasonInfo
    {
        public int? seasonNumber { get; set; }
        public int? episodesCount { get; set; }

		public override bool Equals(object obj)
		{
			SeasonInfo other = obj as SeasonInfo;

			if (other != null)
			{
				foreach (var item in this.GetType().GetProperties())
				{
					if (item.GetValue(this) != item.GetValue(other))
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}