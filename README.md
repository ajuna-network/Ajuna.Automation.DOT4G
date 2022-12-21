# Ajuna.Automation.DOT4G

Τhis repository implements a Bot that can play the DOT4G game (extended version of connect four) over the Ajuna.SDK. 

You can fire up two or more bots that play against each other. 

A video of how it works can be found [here](https://www.loom.com/share/f3afe398aeac4a0ab239a122ea94d0a0).

If you want to try it out, here is what you will need the following:

### Setting up the Ajuna Node and the Trusted Execution Environment

Setup the backend including the Ajuna Node and the Trusted Execution Environment.

 ```batchfile
git clone https://github.com/ajuna-network/backend-devel.git
git checkout release/bengal
cd backend-devel
git submodule init
git submodule update
docker-compose build ajuna-node
docker-compose build worker
```

Register at NGROK and get a free auth token and fill it in the file .env of the repo. 

```batchfile
#In order to use ngrok paste your AUTH TOKEN here
AUTHTOKEN=
 ```

Build the nodes

```batchfile
DOCKER_BUILDKIT=1 COMPOSE_DOCKER_CLI_BUILD=1 docker-compose build ajuna-node
DOCKER_BUILDKIT=1 COMPOSE_DOCKER_CLI_BUILD=1 docker-compose build worker
```

Once you are done, run the nodes

```batchfile
docker-compose up
```

### Getting the information so that you can run the Bot

Check the worker's log to find the following information that we will need in our code:

The MRENCLAVE
```batchfile
decoded-worker-1      | MRENCLAVE=Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi
```

The NGROK Url of the worker
```batchfile
decoded-worker-1      | t=2022-07-06T11:42:08+0000 lvl=info msg="started tunnel" obj=tunnels name=command_line addr=https://localhost:2000 url=https://4f20-84-75-48-249.ngrok.io
```

Once you have this info, you will have to replace it in `Program.cs`:
```batchfile
 private const string WORKER_URL = "ws://991f-89-210-82-26.ngrok.io";
 private const string MRENCLAVE = "AzGcagSmx9ThfFV1D5xwDdnEQfHEGAz5T8A3ivB1FAMx";
 ```

There is the option of starting a single bot to play or multiple using the following two parameters in `Program.cs`:

```batchfile
// Set to true for Stress Testing
 private static bool IS_STRESS_TESTING = true;
 // Sets the maximum number of concurrent bots to be started
 private const int MAX_NUMBER_OF_CONCURRENT_BOTS = 25;
 ```


Now, we have everything we need to launch multiple bots to play.


