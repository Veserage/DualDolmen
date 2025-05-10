using DualDolmen.Exercises.Pages;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DualDolmen.Exercises
{
	public class UserData
	{
		public List<int> FinishedLevels { get; set; }
		public string TimePassed { get; set; }
		public int WordsLearned { get; set; }
	}

	public class GameManager
	{
		private List<Exercise> exercises;
		private int currentExerciseIndex = 0; // Индекс упражнения в пределах уровня номер levelNumber
		public string currentUsername;
		private UserData userData;
		private int levelNumber;

		public GameManager(int levelNumber, string CurrentUsername)
		{
			this.levelNumber = levelNumber;

			var jsonLevel = File.ReadAllText($"Levels/Level{levelNumber}.json");
			var levelData = System.Text.Json.JsonSerializer.Deserialize<LevelData>(jsonLevel);
			
			exercises = levelData.Exercises;

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
                    WordsLearned = 0
                };
				// Пока ничего в файл не сохраняем, ждём когда пользователь пройдёт УРОВЕНЬ
            }
				
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
		public void MarkLevelAsCompleted() {
			userData.FinishedLevels.Add(levelNumber);
			
			string jsonUserData = JsonConvert.SerializeObject(userData, Formatting.Indented);
			File.WriteAllText($"UsersData/{currentUsername}_data.json", jsonUserData);
		}
	}
	// TODO: Нужно сохранение прогресса, и отображение статы в ЛК, еще пройденные уровни красить зеленым в меню выбора уровней
}
