# AlexaBot
A Multi-functional Discord Bot

## Instructions
1. Make sure you have **.NET Core 3+** installed.
2. Set the [**environment variables**](https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=multiservice%2Cwindows#configure-an-environment-variable-for-authentication) for discord secret.
    * Windows(cmd/ps) - setx AlexaBotToken "YOUR_TOKEN_HERE"
    * Linux(bash) - export AlexaBotToken=YOUR_TOKEN_HERE
    * MacOS(bash) - export AlexaBotToken=YOUR_TOKEN_HERE
3. Download [**FFmpeg**](https://ffmpeg.zeranoe.com/builds/) and place it in the executable folder after build.
4. Download [**youtube-dl**](https://ytdl-org.github.io/youtube-dl/download.html) and place it in the executable folder after build.
5. Make sure to download [**opus and libsodium**](https://discord.foxbot.me/docs/guides/voice/sending-voice.html) and place them in the executable folder after build.
6. Run the executable or do **dotnet run** in the project folder.
