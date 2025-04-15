# Roulette Casino Game

A Unity-based casino roulette game with realistic betting mechanics and customizable outcomes.


## Features

- Realistic 3D roulette wheel with dynamic ball physics
- Multiple bet types including straight, split, street, corner, and more
- Persistence of player stats and bets between sessions
- Deterministic outcome selection
- Visual feedback for winning bets with particles and sound effects

## Controls and Gameplay Instructions

### Placing Bets
1. Select your chip value using the chip selection buttons
2. Click on any betting area on the roulette table to place a bet
3. Place multiple bets of different types to diversify your strategy
4. Press the "Play" button to start the roulette wheel

### Special Controls
- **Arrow Button (Top Right)**: Opens a panel that allows you to select the next roulette outcome deterministically
- **Chip Button**: Adds free chips to your balance
- **Trash Can Button**: Removes all your current bets from the table

### Bet Types
- **Straight**: Bet on a single number (Pays 35:1)
- **Split**: Bet on two adjacent numbers (Pays 17:1)
- **Street**: Bet on three numbers in a row (Pays 11:1)
- **Corner**: Bet on four numbers that form a square (Pays 8:1)
- **Six Line**: Bet on six numbers that form two adjacent rows (Pays 5:1)
- **Column**: Bet on all 12 numbers in a column (Pays 2:1)
- **Dozen**: Bet on 12 consecutive numbers (Pays 2:1)
- **Red/Black**: Bet on all red or all black numbers (Pays 1:1)
- **Even/Odd**: Bet on all even or all odd numbers (Pays 1:1)
- **High/Low**: Bet on numbers 1-18 or 19-36 (Pays 1:1)

## Known Issues and Future Improvements

### Known Issues
- The winning number is not visually highlighted after the ball lands, making it difficult to identify
- There is no undo function for individual bet placement - currently you need to remove all bets if you make a mistake

### Planned Improvements
- Add visual highlighting for the winning number for better user experience
- Implement a "Redo" or "Undo" button for more granular control over bet placement
- Add animation and visual effects for chip movement when placing bets
- Add support for both European and American roulette versions (single vs double zero) (could not find a suitable asset for this)
- Implement a more detailed statistics tracking system with graphical representation
- Add a tutorial mode for new players
- Improve mobile responsive design for better playability on different devices
- Implement a leaderboard and achievement system

## Demo

[Watch the Roulette Game Demo](https://drive.google.com/file/d/14B4Fhpq0E3tj-hDmUbYZXxqPY3uNMpoq/view?usp=sharing)

## OOP Principles Applied

### Encapsulation
- Private member variables with public accessor methods protect data integrity
- Classes like `UserMoney` and `BetController` encapsulate their behavior and state
- Game phases and state transitions are encapsulated within the `GameLoop` class

### Inheritance
- Various bet types (StraightBet, SplitBet, CornerBet, etc.) inherit from a common base
- UI components use Unity's inheritance hierarchy for consistent behavior

### Polymorphism
- The betting system uses polymorphism through interfaces like `IBetButton`
- Different bet types implement their own `IsWinner` and `CalculatePayout` methods
- The roulette system allows for different outcome providers through the `IRouletteOutcomeProvider` interface

### Abstraction
- Complex betting rules and payout calculations are abstracted away from the main game flow
- The user interface abstracts the underlying data structures and algorithms
- Game states are represented as abstract concepts (phases) rather than implementation details

### SOLID Principles
1. **Single Responsibility Principle**: Classes like `RouletteController`, `BetController`, and `UIManager` have well-defined responsibilities
2. **Open/Closed Principle**: New bet types can be added without modifying existing code
3. **Liskov Substitution Principle**: Child classes like specific bet types can be used anywhere their parent types are expected
4. **Interface Segregation**: Small, focused interfaces like `IBetButton` and `IRouletteOutcomeProvider`
5. **Dependency Inversion**: High-level modules depend on abstractions, not concrete implementations

## Technical Details

### Architecture
The project follows a component-based architecture with clear separation of concerns:
- **Game**: Core game loop and state management
- **Roulette**: Wheel mechanics and outcome determination
- **Betting**: Bet placement, validation
- **User**: Player data, money management, and persistence
- **UI**: User interface components and interaction handling

### Persistence
Player stats, money, and active bets are saved automatically between sessions using a serialization system implemented in the `PlayerSave` class.

## License