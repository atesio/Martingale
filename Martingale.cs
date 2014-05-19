using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martingale
{
    public struct Turn
    {
        public double Bet;
        public bool Won;
    }
    public class Game
    {
        public int ID { get; private set; }
        public double StartingMoney { get; private set; }
        public double PL { get; set; }
        public List<Turn> Turns;

        public Game(int id, double startingMoney)
        {
            ID = id;
            StartingMoney = startingMoney;
            PL = 0;
            Turns = new List<Turn>();
        }
        public string Details
        {
            get
            {
                return String.Join(", ", from t in Turns select (t.Won ? "+" : "-")+ t.Bet.ToString());
            }
        }
    }
    public class Session
    {
        public SessionParameters Parameters;
        public List<Game> Games;
        public double PL;

        public Session(SessionParameters parameters)
        {
            Parameters = parameters;
            Games = new List<Game>();
            PL = 0;
        }
        public double RemainingMoney(Game game = null)
        {
            return Parameters.StartMoney + PL + (game==null ? 0 : game.PL);
        }
        public bool CanPlayAnotherGame()
        {
            return RemainingMoney(null) >= Parameters.MinBet && Games.Count < Parameters.MaxGames;
        }
        private double ComputeNextBet(Game game)
        {
            if (game.Turns.Count == 0)
                return Parameters.MinBet;
            else
                return game.Turns.Last().Bet * Parameters.BetIncrease;
        }
        public double GetEffectiveWinPercent()
        {
            double nbTotal = (from g in Games select g.Turns.Count).Sum();
            double nbLosses = (from g in Games select g.Turns.Where((turn => !turn.Won)).Count()).Sum();
            return 100.0 - 100.0 * nbLosses / nbTotal;
        }
        public void PlayGame()
        {
            var game = new Game(Games.Count+1, RemainingMoney());
            game.PL = 0;
            game.Turns = new List<Turn>();
            while (true)
            {
                var bet = ComputeNextBet(game);
                if (bet > RemainingMoney(game)) break;
                if (bet > Parameters.MaxBet) break;
                bool won = Player.GetRandomOutcome(Parameters.WinPercent);
                var turn = new Turn() { Bet = bet, Won = won };
                game.Turns.Add(turn);
                game.PL += bet * (won ? 1 : -1);
                if (won) break;
            }
            Games.Add(game);
            PL += game.PL;
        }
    }
    public class SessionParameters
    {
        public double StartMoney { get; set; }
        public double MinBet  { get; set; } // first bet of a game
        public double BetIncrease { get; set; } // if loss, play again with the previous bet multiplied by this ratio, eg. 2
        public double MaxBet { get; set; } // don't bet more than this
        public int MaxGames { get; set; } // stop after playing this number of games
        public double WinPercent  { get; set; } // your percentage of chances to win a game, eg. 50
    }

    public static class Player
    {
        public static Random random = new Random();
        public static bool GetRandomOutcome(double winChances)
        {
            return random.NextDouble() < (winChances/100.0);
        }
        public static Session Play(SessionParameters parameters)
        {
            var session = new Session(parameters);
            while (session.CanPlayAnotherGame()) session.PlayGame();
            return session;
        }
    }
}
