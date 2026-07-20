# Prompts & AI Tooling Usage

## Tool Used

- **GitHub Copilot CLI** — IDE-integrated AI tool used during implementation

## Usage

- AI tooling was used across the SDLC — analysis, modelling, tests, and documentation
- IDE-integrated tool was actively used during implementation
- Significant prompts and key judgement calls are captured below

## Key Prompts

1. Initial project scaffolding and architecture design based on the challenge document
2. Data model design for unified FlightStatusResult and provider response models
3. Status vocabulary mapping between AeroTrack/QuickFlight and unified enum
4. Merge rule implementation (latest timestamp wins)
5. Deterministic stub data covering all status scenarios
6. Unit test generation for normalisation, merge, and provider logic
7. Angular frontend with colour-coded status badges
