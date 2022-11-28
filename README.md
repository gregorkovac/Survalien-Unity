# Survalien - Unity

## Odpiranje projekta
Uvozi samo mapo `Survalien/`. Ostale datoteke (`Models`, `Textures`, ...) so samo za hranjenje assetov in niso povezane s projektom.

## Razlaga map v Unity projektu

- `Scenes` - scene (game, main menu, ...)
- `Scripts` - skripte oz. koda, ki jo dodamo objektom v sceni
- `Models` - "raw" modeli (`.obj`, `,fbx`, ...)
- `Textures` - "raw" teksture (`.png`, `.jpeg`, ...); če je model v formatu `.fbx`, najbrž ne bo treba eksplicitno dodajat teksture
- `Materials` - materiali, ki jih naredimo iz tekstur in jih lahko dodamo modelom
- `Prefabs` - game objecti, ki so shranjeni in jih lahko kasneje uporabimo ali kopiramo. Naprimer metek.