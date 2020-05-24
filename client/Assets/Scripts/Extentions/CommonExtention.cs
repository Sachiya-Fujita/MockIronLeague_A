using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public static partial class CommonExtention
{
    #region String

    /// <summary>
		/// 文字列の1行目を取得する
		/// </summary>
		public static string GetFirstLine(this string self)
		{
			var separator = new [] { System.Environment.NewLine };

			return self
				.Split(separator, System.StringSplitOptions.None)
				.FirstOrDefault();
		}

    #endregion

}
