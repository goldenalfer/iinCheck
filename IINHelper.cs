using System;
using System.Linq;
using System.Text.RegularExpressions;

public class IINHelper
{
	public DateTime? BirthDate { get; private set; }
	public IINSexEnum IINSex { get; private set; }
	public bool IsPerson { get; private set; }
	public bool IsValid { get; private set; }
	public string ErrorMessage { get; private set; }

	public IINHelper()
	{ }

	public IINHelper Check(string iin)
	{
		if (iin == null)
		{
			throw new ArgumentException("[iin] не может быть null");
		}

		this.IsValid = false;
		this.ErrorMessage = null;
		this.BirthDate = null;
		this.IINSex = IINSexEnum.UNSPECIFIED;

		if (iin.Length != 12)
		{
			this.ErrorMessage = "Некорректная длина ИИН. Должна быть 12 символов";
			return this;
		}

		if (!new Regex("[0-9]{12}").IsMatch(iin))
		{
			this.ErrorMessage = "Некорректные символы в ИИН. Должны быть только цифры";
			return this;
		}

		var digits = iin.Select(v => int.Parse(v.ToString())).ToArray();

		//Определяем физ лицо или юр лицо ([6] символ 0 - юр лицо, остальные физ лицо)
		this.IsPerson = digits[6] != 0;

		if (this.IsPerson)
		{
			//Для физ лиц определяем пол и дату рождения
			//Пол ([6] символ - нечетный - мужчина, четный - женщина
			if (digits[6] % 2 == 1)
			{
				this.IINSex = IINSexEnum.MALE;
			}
			else
			{
				this.IINSex = IINSexEnum.FEMALE;
			}

			//Дата рождения ([0][1] - последние 2 цифры года рождения, [2][3] - месяц рождения, [4][5] - день рождения, [6] - 1,2 - 19 век, 3,4 - 20 век, 5,6 - 21 век
			try
			{
				this.BirthDate = new DateTime((((int)((digits[6] - 1) / 2)) + 18) * 100 + digits[0] * 10 + digits[1], digits[2] * 10 + digits[3], digits[4] * 10 + digits[5]);
			}
			catch (ArgumentOutOfRangeException)
			{
				this.ErrorMessage = "Некорректный ИИН. Ошибка в первых 7 символах";
				return this;
			}
		}
		else
		{
			this.IINSex = IINSexEnum.UNSPECIFIED;
		}

		//Проверяем контрольный разряд [11]
		var checkDigits1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
		var checkDigits2 = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 1, 2 };
		int c1 = 0, c2 = 0;
		for (int i = 0; i < 11; i++)
		{
			c1 = c1 + digits[i] * checkDigits1[i];
			c2 = c2 + digits[i] * checkDigits2[i];
		}
		var c = c1 % 11;
		if (c1 == 10)
		{
			if (c2 % 10 == 10)
			{
				this.ErrorMessage = "Некорректный ИИН. Данный ИИН не используется";
				return this;
			}
			c = c2 % 10;
		}

		if (c != digits[11])
		{
			this.ErrorMessage = "Некорректный ИИН. Не совпадает контрольный разряд";
			return this;
		}

		this.IsValid = true;

		return this;
	}
}

public enum IINSexEnum
{
	UNSPECIFIED = 0,
	MALE = 1,
	FEMALE = 2
}