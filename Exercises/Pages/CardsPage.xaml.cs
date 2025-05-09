using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DualDolmen.Exercises.Pages
{
	/// <summary>
	/// Interaction logic for CardsPage.xaml
	/// </summary>
	public partial class CardsPage : Page
	{
		private List<CardItem> cards = new();
		private GameManager gameManager;
		private Exercise exercise;

		private Button firstSelectedCard;
		private CardItem firstSelectedItem;

		private int totalPairCount = 0; // Количество пар всего
		private int pairCount = 0; // Количество пар выбрано

		public CardsPage (Exercise exercise, GameManager gameManager)
		{
			InitializeComponent();
			this.exercise = exercise;
			this.gameManager = gameManager;
			InitCards();
		}

		private void InitCards()
		{
			// Заполнение карточек из данных упражнения
			foreach (var pairElement in exercise.Content.EnumerateArray())
			{
				string term = pairElement.GetProperty("Term").GetString();
				string match = pairElement.GetProperty("Match").GetString();

				// term связывает слово и его перевод, из 3 строк json получаем 6 карточек, из 4 строк - 8 карточек и т.д.
				cards.Add(new CardItem { Text = term, PairId = term});
				cards.Add(new CardItem { Text = match, PairId = term});

				totalPairCount++;
			}


			Shuffle(cards);

			foreach (var card in cards)
			{
				var button = new Button
				{
					Content = card.Text,
					Tag = card,
					Margin = new Thickness(15),
					FontSize = 20,
					Background = new SolidColorBrush(Colors.Snow)
				};
				button.Click += Card_Click;
				CardGrid.Children.Add(button);
			}
		}

		private void Card_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var card = (CardItem)button.Tag;

			if (firstSelectedCard == null)
			{
				firstSelectedCard = button;
				firstSelectedItem = card;
				button.Background = Brushes.LightBlue;
			}
			else if (button != firstSelectedCard)
			{
				// Второй клик
				if (firstSelectedItem.PairId == card.PairId)
				{
					// Совпадение
					button.Background = Brushes.LightGreen;
					firstSelectedCard.Background = Brushes.LightGreen;

					button.Visibility = Visibility.Hidden;
					firstSelectedCard.Visibility = Visibility.Hidden;
					pairCount++;
				}
				else
				{
					// Ошибка
					button.Background = Brushes.IndianRed;
					firstSelectedCard.Background = Brushes.IndianRed;
					//gameManager.RegisterFailure(gameManager.CurrentExerciseIndex);
				}

				// Очистка выбора
				Task.Delay(500).ContinueWith(_=>
				{
					Dispatcher.Invoke(() =>
					{
						button.Background = Brushes.White;

						if (firstSelectedCard != null)
							firstSelectedCard.Background = Brushes.White; // Card - это на самом деле Button

						firstSelectedCard = null;
						firstSelectedItem = null;

						CheckIfCompleted();
					});
				});
			}
		}

		private void CheckIfCompleted()
		{
			if (pairCount == totalPairCount)
			{
				gameManager.AdvanceExercise();
				NavigationService.Navigate(gameManager.GetCurrentExercisePage());
			}
		}

		// Перетасовка карточек
		private void Shuffle<T>(IList<T> list)
		{
			var rng = new Random();
			for (int i = 0; i < list.Count; i++)
			{
				int j = rng.Next(i, list.Count);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}

	public class CardItem
	{
		public string Text { get; set; }
		public string PairId { get; set; } // идентификатор пары
	}

}
