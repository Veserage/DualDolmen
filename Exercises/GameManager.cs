using DualDolmen.Exercises.Pages;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DualDolmen.Exercises
{
	public class UserData
	{
		public List<int> FinishedLevels { get; set; }
		public string TimePassed { get; set; }
		public int WordsLearnedCount { get; set; }
	}

	public class GameManager
	{
		public List<Exercise> exercises;
		public int currentExerciseIndex = 0; // Индекс упражнения в пределах уровня номер levelNumber
		public string currentUsername;
		private UserData userData;
		public int levelNumber;
		private DateTime levelStartTime;
		private int completedWordsCountForLevel = 0;

		public GameManager(int levelNumber, string CurrentUsername)
		{
			this.levelNumber = levelNumber;

			if (!File.Exists($"Levels/Level{levelNumber}.json")) { MessageBox.Show($"Ошибка - не найден файл Level{levelNumber}.json, загрузка уровня не возможна", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); Application.Current.Shutdown(); return; }
			var jsonLevel = File.ReadAllText($"Levels/Level{levelNumber}.json");
			var levelData = System.Text.Json.JsonSerializer.Deserialize<LevelData>(jsonLevel);
			
			exercises = levelData.Exercises;
			completedWordsCountForLevel = levelData.CompletedWordsCount;

			this.currentUsername = CurrentUsername;

			// Если есть информация о прогрессе пользователя, загружаем ее
			if (File.Exists($"UsersData/{currentUsername}_data.json"))
			{
				string jsonUserData = File.ReadAllText($"UsersData/{currentUsername}_data.json");
				userData = System.Text.Json.JsonSerializer.Deserialize<UserData>(jsonUserData);
            }
			else // Если пользователь еще не прошел ни одного уровня
			{
                this.userData = new UserData
                {
                    FinishedLevels = new List<int>(),
                    TimePassed = "",
                    WordsLearnedCount = 0
                };
				// Пока ничего в файл не сохраняем, ждём когда пользователь пройдёт УРОВЕНЬ
            }

			this.levelStartTime = DateTime.Now;

		}

		public Page GetCurrentExercisePage()
		{
			var exercise = exercises[currentExerciseIndex];
			return exercise.Type switch
			{
				"Cards" => new CardsPage(exercise, this),
				"OddOne" => new OddOnePage(exercise, this),
				"InsertWord" => new InsertWordPage(exercise, this),
				_ => throw new Exception($"Неизвестный тип упражнения: {exercise.Type}")
			};
		}

		public void AdvanceExercise() => currentExerciseIndex++;

		public bool HasMoreExercises => currentExerciseIndex < exercises.Count - 1; // "- 1 потому, что начинаем индексацию упражнений с нуля"

		// Сохранение прогресса в личный data.json пользователя
		public void MarkLevelAsCompleted()
		{
			// Увеличиваем количество выученных слов
			if (!userData.FinishedLevels.Contains(levelNumber)) { userData.WordsLearnedCount += completedWordsCountForLevel; }

			

				// Подсчёт времени
			TimeSpan sessionDuration = DateTime.Now - levelStartTime;

			TimeSpan totalTimePassed = TimeSpan.Zero;
			if (TimeSpan.TryParse(userData.TimePassed, out var parsed))
				totalTimePassed = parsed;

			totalTimePassed += sessionDuration;
			userData.TimePassed = totalTimePassed.ToString(@"hh\:mm\:ss");

			// Добавление уровня в пройденные
			if (!userData.FinishedLevels.Contains(levelNumber)) { userData.FinishedLevels.Add(levelNumber); }

			// Сохраняем
			string jsonUserData = JsonConvert.SerializeObject(userData, Formatting.Indented);
			File.WriteAllText($"UsersData/{currentUsername}_data.json", jsonUserData);
		}


	}

}
