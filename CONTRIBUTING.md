# Adding to the Rock Engine

Thank you for considering contributing to the Rock, this document is a simple brief as to how to go about doing that.

To start adding your own code, check the pull requests for the repository to check if someone isn't already working on the feature you want to add, if so, join their pull request and help out.

If you open a pull request, please try to be active and reachable by the maintainers so we can help build your feature into the engine.

## Guidelines

- All code in this repository is linted with [Super Linter](https://github.com/super-linter/super-linter), that includes shell scripts as well as spelling (which is also checked with [Check Spelling](https://github.com/check-spelling/check-spelling)). This linter uses the `.editorconfig` rules in the project root, so configure your IDE to use that for code style.

- Your code MUST pass all checks to be merged.

- Your code MUST be entirely your own, or governed by a license compatible with `LGPLv3` (which also means you need to accredit the original source and supply their license in a comment).

- Do try to add Doxygen comments where necessary to all code you write.

## What we are looking for

### Documentation

This project uses Doxygen to generate docs for our GitHub pages website, the engine code is only partially documented however. You could read up on how to write Doxygen code comments and help us document the engine API.

### Editor tooling

Rock as of currently is a library, not an integrated development environment with a graphical editor like Unity. Our current goal is to bring basic editor tooling to Rock using [Avalonia](https://www.avaloniaui.net/).

The first component will be a level builder which leverages and initializes our `Scene`/`Node`/`Behaviour` classes. This would allow you to compose scene hierarchy of Nodes in a tree, assign Behaviours to Nodes, and assign initialization values to their public fields.

### 3D renderer

Our current `PebbleRenderer` is strictly 2D, we plan to build a new rendering backend which is more flexible.

## Bugs and feature suggestions

Please submit an issue on this repository, making use of the respective issue template.

## Support

Do not use issues for submitting support and help requests, please use the repository discussions page.
