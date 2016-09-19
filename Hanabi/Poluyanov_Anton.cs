using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanabi
{
    class Poluyanov_Anton
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            string input;

            while ((input = Console.ReadLine()) != null)
            {
                Command inputCommand = new Command(input);
                game.ExecuteCommand(inputCommand);
            }   
        }
    }

    class Card
    {
        public char Color { get; set; }
        public int Rank { get; set; }
        public List<Card> GuessesOfWhatCardItCanBe { get; set; }  

        public Card(string cardValue)
        {
            Color = cardValue[0];
            Rank = Int32.Parse(cardValue[1].ToString());
            char[] possibleColors = new[] {'R', 'G', 'B', 'Y', 'W'};
            int[] possibleRanks = new[] {1, 2, 3, 4, 5};
            var possibleCardValues =
                from color in possibleColors
                from rank in possibleRanks
                select new Card(color, rank);
            GuessesOfWhatCardItCanBe = possibleCardValues.ToList();
        }

        public Card(char color, int rank)
        {
            Color = color;
            Rank = rank;
        }

        public override bool Equals(object obj)
        {
            Card card = (Card)obj;
            return ((Color == card.Color) && (Rank == card.Rank));
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Color) + Rank;
        }
    }

    struct Command
    {
        public string CommandName { get; set; }
        public char Color { get; set; }
        public int Rank { get; set; }
        public List<string> CardIndexes { get; set; }

        static char GetColor(List<string> colorAndCardIndexes)
        {
            return colorAndCardIndexes[0][0];
        }

        static int GetRank(List<string> rankAndCardIndexes)
        {
            return int.Parse(rankAndCardIndexes[0]);
        }

        static List<string> GetCardIndexes(List<string> rankOrColorAndCardIndexes)
        {
            rankOrColorAndCardIndexes.RemoveAt(0);
            return rankOrColorAndCardIndexes;
        }

        public Command(string commandFromConsole)
        {
            Color = '0';
            Rank = 0;

            if (commandFromConsole.StartsWith("Start"))
            {
                CommandName = "start";
                CardIndexes = commandFromConsole.Replace("Start new game with deck ", "").Split().ToList();
            }
            else if (commandFromConsole.StartsWith("Tell color"))
            {
                List<string> rankOrColorAndCardIndexes = GetOnlyRankOrColorAndCardIndexes(commandFromConsole);

                CommandName = "tellColor";
                Color = GetColor(rankOrColorAndCardIndexes);
                this.CardIndexes = GetCardIndexes(rankOrColorAndCardIndexes);
            }
            else if (commandFromConsole.StartsWith("Tell rank"))
            {
                List<string> rankOrColorAndCardIndexes = GetOnlyRankOrColorAndCardIndexes(commandFromConsole);

                CommandName = "tellRank";
                Rank = GetRank(rankOrColorAndCardIndexes);
                this.CardIndexes = GetCardIndexes(rankOrColorAndCardIndexes);
            }
            else if (commandFromConsole.StartsWith("Play"))
            {
                CommandName = "play";
                CardIndexes = new List<string> { commandFromConsole.Split()[2] };
            }
            else
            {
                CommandName = "drop";
                CardIndexes = new List<string> { commandFromConsole.Split()[2] };
            }
        }

        static List<string> GetOnlyRankOrColorAndCardIndexes(string commandFromConsole)
        {
            List<string> rankOrColorAndCardIndexes = commandFromConsole.Split().ToList();
            rankOrColorAndCardIndexes.RemoveRange(0, 2);
            rankOrColorAndCardIndexes.RemoveRange(1, 2);
            return rankOrColorAndCardIndexes;
        }
    }

    class PlayedCards
    {
        public int RankOfLastPlayedRedCard { get; set; }
        public int RankOfLastPlayedGreenCard { get; set; }
        public int RankOfLastPlayedBlueCard { get; set; }
        public int RankOfLastPlayedYellowCard { get; set; }
        public int RankOfLastPlayedWhiteCard { get; set; }
        public List<Card> CardsWhichArePossibleToAdd { get; set; }
        public int Count => RankOfLastPlayedRedCard + RankOfLastPlayedGreenCard + RankOfLastPlayedBlueCard +
                            RankOfLastPlayedYellowCard + RankOfLastPlayedWhiteCard;

        public PlayedCards()
        {
            RankOfLastPlayedRedCard = 0;
            RankOfLastPlayedGreenCard = 0;
            RankOfLastPlayedBlueCard = 0;
            RankOfLastPlayedYellowCard = 0;
            RankOfLastPlayedWhiteCard = 0;
            CardsWhichArePossibleToAdd = new List<Card> {new Card("R1"), new Card("G1"), new Card("B1"), new Card("Y1"), new Card("W1")};
        }

        void ChangePossibleCardToAddToCardOfHigherRank(Card card)
        {
            CardsWhichArePossibleToAdd.Remove(card);
            if (card.Rank + 1 <= 5)
                CardsWhichArePossibleToAdd.Add(new Card(card.Color, card.Rank + 1));
        }

        public void AddCard(Card card)
        {
            switch (card.Color)
            {
                case 'R':
                    RankOfLastPlayedRedCard++;
                    break;
                case 'G':
                    RankOfLastPlayedGreenCard++;
                    break;
                case 'B':
                    RankOfLastPlayedBlueCard++;
                    break;
                case 'Y':
                    RankOfLastPlayedYellowCard++;
                    break;
                case 'W':
                    RankOfLastPlayedWhiteCard++;
                    break;
            }
            ChangePossibleCardToAddToCardOfHigherRank(card); 
        }
    }

    class Game
    {
        public int Turn { get; set; }
        public int RiskyCardsCount { get; set; }
        public List<string> Deck { get; set; }
        public PlayedCards PlayedCards { get; set; }
        public List<Card> CurrentPlayerCards { get; set; }
        public List<Card> NextPlayerCards { get; set; }
        public bool Finished { get; set; }

        public Game()
        {
            Turn = 0;
            RiskyCardsCount = 0;
            Finished = false;
        }

        void StartNewGame(List<string> cards)
        {
            Turn = 0;
            RiskyCardsCount = 0;
            Finished = false;

            CurrentPlayerCards = new List<Card>(5);
            NextPlayerCards = new List<Card>(5);
            Deck = new List<string>();
            PlayedCards = new PlayedCards();

            for (int i = 0; i < 5; i++)
                CurrentPlayerCards.Add(new Card(cards[i]));

            for (int i = 5; i < 10; i++)
                NextPlayerCards.Add(new Card(cards[i]));

            for (int i = 10; i < cards.Count; i++)
                Deck.Add(cards[i]);
        }

        public void ExecuteCommand(Command command)
        {
            List<int> cardIndexesFromCommand;

            switch (command.CommandName)
            {
                case "start":
                    StartNewGame(command.CardIndexes.ToList());
                    break;
                case "play":
                    if (!Finished)
                        PlayCard(int.Parse(command.CardIndexes[0]));
                    break;
                case "drop":
                    if (!Finished)
                        DropCard(int.Parse(command.CardIndexes[0]));
                    break;
                case "tellColor":
                    if (!Finished)
                    {
                        cardIndexesFromCommand = command.CardIndexes.Select(x => int.Parse(x)).ToList();
                        TellColor(command.Color, cardIndexesFromCommand);
                    }
                    break;
                case "tellRank":
                    if (!Finished)
                    {
                        cardIndexesFromCommand = command.CardIndexes.Select(x => int.Parse(x)).ToList();
                        TellRank(command.Rank, cardIndexesFromCommand);
                    }
                    break;
            }
        }

        void TakeCardFromDeck()
        {
            CurrentPlayerCards.Add(new Card(Deck[0]));
            Deck.RemoveAt(0);
        }

        void SwitchToNextPlayer()
        {
            var variableForSwapping = CurrentPlayerCards;
            CurrentPlayerCards = NextPlayerCards;
            NextPlayerCards = variableForSwapping;
        }

        bool IsCorrectCard(Card card)
        {
            if (PlayedCards.CardsWhichArePossibleToAdd.Contains(card))
                return true;
            return false;
        }
        
        bool IsRiskyCard(Card card)
        {
            return !card.GuessesOfWhatCardItCanBe.TrueForAll(x => PlayedCards.CardsWhichArePossibleToAdd.Contains(x));
        }

        bool IsCorrectNumberAndIndexesOfCardsOfGivenColor(char colorFromCommand, List<int> cardIndexesFromCommand)
        {
            bool numberAndIndexesOfCardsOfGivenColorAreCorrect = true;

            int cardsOfColorFromCommandCount = NextPlayerCards.Count(x => x.Color == colorFromCommand);
            if (cardIndexesFromCommand.Count != cardsOfColorFromCommandCount)
                numberAndIndexesOfCardsOfGivenColorAreCorrect = false;

            if (cardIndexesFromCommand.Any(playerCardIndex => NextPlayerCards[playerCardIndex].Color != colorFromCommand))
            {
                numberAndIndexesOfCardsOfGivenColorAreCorrect = false;
            }

            return numberAndIndexesOfCardsOfGivenColorAreCorrect;
        }

        bool IsCorrectNumberAndIndexesOfCardsOfGivenRank(int rankFromCommand, List<int> cardIndexesFromCommand)
        {
            bool numberAndIndexesOfCardsOfGivenRankAreCorrect = true;

            int cardsOfRankFromCommandCount = NextPlayerCards.Count(x => x.Rank == rankFromCommand);
            if (cardIndexesFromCommand.Count != cardsOfRankFromCommandCount)
                numberAndIndexesOfCardsOfGivenRankAreCorrect = false;

            if (cardIndexesFromCommand.Any(playerCardIndex => NextPlayerCards[playerCardIndex].Rank != rankFromCommand))
            {
                numberAndIndexesOfCardsOfGivenRankAreCorrect = false;
            }

            return numberAndIndexesOfCardsOfGivenRankAreCorrect;
        }

        void PutCardOnTable(int cardIndex)
        {
            PlayedCards.AddCard(CurrentPlayerCards[cardIndex]);
            CurrentPlayerCards.RemoveAt(cardIndex);
        }

        void ExcludeWrongGuessesOfColorOfCardsWithIndexesFromCommand(char colorFromCommand, List<int> cardIndexesFromCommand)
        {
            foreach (var cardIndex in cardIndexesFromCommand)
            {
                NextPlayerCards[cardIndex].GuessesOfWhatCardItCanBe.RemoveAll(
                    x => x.Color != colorFromCommand);
            }
        }

        void ExcludeWrongGuessesOfColorOfCardsWithIndexesNotIncludedInCommand(char colorFromCommand, List<int> cardIndexesFromCommand)
        {
            List<int> notIncludedInCommandCardIndexes = new List<int> { 0, 1, 2, 3, 4 };
            notIncludedInCommandCardIndexes =
                notIncludedInCommandCardIndexes.Where(x => !cardIndexesFromCommand.Contains(x)).ToList();

            foreach (var cardIndex in notIncludedInCommandCardIndexes)
            {
                NextPlayerCards[cardIndex].GuessesOfWhatCardItCanBe.RemoveAll(x => x.Color == colorFromCommand);
            }
        }

        void ExcludeWrongGuessesOfRankOfCardsWithIndexesFromCommand(int rankFromCommand, List<int> cardIndexesFromCommand)
        {
            foreach (var cardIndex in cardIndexesFromCommand)
            {
                NextPlayerCards[cardIndex].GuessesOfWhatCardItCanBe.RemoveAll(
                    x => x.Rank != rankFromCommand);
            }
        }

        void ExcludeWrongGuessesOfRankOfCardsWithIndexesNotIncludedInCommand(int rankFromCommand, List<int> cardIndexesFromCommand)
        {
            List<int> notIncludedInCommandCardIndexes = new List<int> { 0, 1, 2, 3, 4 };
            notIncludedInCommandCardIndexes =
                notIncludedInCommandCardIndexes.Where(x => !cardIndexesFromCommand.Contains(x)).ToList();

            foreach (var cardIndex in notIncludedInCommandCardIndexes)
            {
                NextPlayerCards[cardIndex].GuessesOfWhatCardItCanBe.RemoveAll(x => x.Rank == rankFromCommand);
            }
        }

        void PlayCard(int cardIndex)
        {
            Turn++;
            if (IsCorrectCard(CurrentPlayerCards[cardIndex]))
            {
                if (IsRiskyCard(CurrentPlayerCards[cardIndex]))
                        RiskyCardsCount++;

                PutCardOnTable(cardIndex);

                if (PlayedCards.Count == 25)
                    FinishGame();
                else
                {
                    if (Deck.Count != 1)
                    {
                        TakeCardFromDeck();
                        SwitchToNextPlayer();
                    }
                    else FinishGame();
                }
            }
            else
                FinishGame();
        }

        void DropCard(int cardIndex)
        {
            Turn++;
            CurrentPlayerCards.RemoveAt(cardIndex);      

            if (Deck.Count != 1)
                TakeCardFromDeck();  
            else
            {
                FinishGame();
                return;
            }

            SwitchToNextPlayer();
        }

        void TellColor(char colorFromCommand, List<int> cardIndexesFromCommand)
        {
            Turn++;

            if (!IsCorrectNumberAndIndexesOfCardsOfGivenColor(colorFromCommand, cardIndexesFromCommand))
                FinishGame();
            else
            {
                ExcludeWrongGuessesOfColorOfCardsWithIndexesFromCommand(colorFromCommand,cardIndexesFromCommand);
                ExcludeWrongGuessesOfColorOfCardsWithIndexesNotIncludedInCommand(colorFromCommand,cardIndexesFromCommand);   
            }
            
            SwitchToNextPlayer();
        }

        void TellRank(int rankFromCommand, List<int> cardIndexesFromCommand)
        {
            Turn++;

            if (!IsCorrectNumberAndIndexesOfCardsOfGivenRank(rankFromCommand, cardIndexesFromCommand))
                FinishGame();
            {
                ExcludeWrongGuessesOfRankOfCardsWithIndexesFromCommand(rankFromCommand, cardIndexesFromCommand);
                ExcludeWrongGuessesOfRankOfCardsWithIndexesNotIncludedInCommand(rankFromCommand, cardIndexesFromCommand);
            }

            SwitchToNextPlayer();
        }

        void FinishGame()
        {
            Console.WriteLine("Turn: " + Turn + ", cards: " + PlayedCards.Count + ", with risk: " + RiskyCardsCount);
            Finished = true;
        }
    }
}
