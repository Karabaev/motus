namespace InfoAgent
{
    using System;
    using System.Collections.Generic;

    public class Translation
    {
        public string studioName { get; set; }
        public List<SeasonInfo> listOfSeasons { get; set; }
        public DateTime? updateTime { get; set; }
        public DateTime? lastEpisodeTime { get; set; }

		public override bool Equals(object obj)
		{
			Translation other = obj as Translation;

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