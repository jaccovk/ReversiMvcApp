
const Game = (function () {

/*
    let stateMap = {
        gameState: 0,
    }
*/
    const _getCurrentGameState = function() {
        Game.Data.stateMap.gameState = Game.Model.getGameState();
    }



    const privateInit = function(spelToken, spelerToken){
        console.log("spelToken", spelToken);
        //add buttons
        Game.Buttons.addButtons(spelToken, spelerToken);

        //add board
        Game.Reversi.initReversi(spelToken, spelerToken);

        //set interval
        setInterval(() => Game.Reversi.updateBoard(), 1000);

        //afterInit();
        console.log("game ge-initialiseerd");
    }

    return {
        init: privateInit,
    }
})('/api/game');
Game.Data = (function(){
    let stateMap = {
        // environment : 'production',
        environment: 'development',
        gameState : 0
    }
    const configMap = {
        isProduction : stateMap.environment === 'production',
        apiUrl: stateMap.isProduction ? "/api/spel/" : "http://localhost:5001/api/spel/",
        /*        mock: [
            {
                url: `api/Spel/Beurt`,
                data: 0
            }
        ],*/
    }

/*    const getMockData = function(){

        const mockData = configMap.mock;

        return new Promise((resolve, reject) => {
            resolve(mockData);
        });

    }*/

/*    const get = function(url){
        if(stateMap.environment === 'development')
            return getMockData(configMap.mock.url);
        else {
            return $.get(url)
                .then(r => {
                    return r
                })
                .catch(e => {
                    console.log(e.message);
                });
        }
    };*/

/*    const publicInit = function(environment){
        //Game.init();
        console.log('init game data ...');
        if(environment === "production" || environment === "development") {
            stateMap.environment = environment;
            //get(url);
        }
        else throw new Error("verkeerde environment");
    }*/

    return {
/*      initData: publicInit,
        get: get,*/
        stateMap: stateMap,
        configMap: configMap
    }
})();

Game.Model = (function () {
    //bevat gegevens spelers/spel
    let configMap = {}
    const publicInit = function () {
        Game.init();
    }

    const getWeather = function (url) {
        Game.Data.get(url).then(d => {


            if (d?.main?.temp != null) {
                console.log(`Temp = ${d.name} is ${d.main.temp}`);
                return d;
            } else {
                throw new Error('jammer jochie')
            }
        }).catch(e => {
            console.log(`Error is ${e.message}`);
            throw new Error('jammer jochie')
        });
    }

    const _getGameState = function () {

        //aanvraag via Game.Data
        var data = Game.Data.get(`api/Spel/Beurt/<token>`);
        //controle of ontvangen data valide is
        if (data === 0 || data === 1 || data === 2) {
            if (data === 0) {
                console.log('*geen waarde*')
            } else if (data === 1) {
                console.log('*wit aan zet*')
            } else if (data === 2) {
                console.log('*zwart aan zet*')
            }
            return data;

        } /*else throw new Error('jammer jochie');*/
    }

    return {
        initModel: publicInit,
        getWeather: getWeather,
        getGameState: _getGameState
    }
})();
Game.Reversi = (function(){
    let configMap = {
        spelToken: "",
        speler: [],
        spel: [],

        afgelopen: false,
        colors: ["geen", "wit", "zwart"],
    };

    const countFichesByColor = (color, bord) => {
        return Game.Stats.countFichesByColor(color, bord);
    }


    const privateInit = async function (spelToken, spelerToken) {
        if (!configMap.afgelopen) {
            configMap.spelToken = spelToken;
            configMap.speler.token = spelerToken;
            await setSpel();

            //place board
            let bord = document.getElementsByClassName('game-board');
            for (let i = 1; i <= configMap.spel.bord.length; i++) {
                for (let j = 1; j <= configMap.spel.bord.length; j++) {
                    //set square
                    let square = setSquare(i, j);
                    bord[0].appendChild(square);
                }
            }

            //set cat facts
            setCatFacts();

            //set chart
            setChart();

            //set stats
            setStats();
        }
    };

    const setCatFacts = async function () {
        let catFacts = await Game.Api.getCatFacts();
        console.log(catFacts.data[0]);
        document.getElementsByClassName('cat-facts')[0].innerText = catFacts.data[0];
    }

    const setStats = function () {
        const gameStats = document.getElementsByClassName('game-stats')[0];
        const stats = Game.Stats.getStats(configMap.spel.bord);
        gameStats.innerHTML = `
            <hr/>
                ${stats.witIsWinning ? "Wit" : "Zwart"}  is aan het winnen.<hr/>
                Er zijn nog ${stats.aantalLegeVelden} lege velden.
            <hr/>
        `;
    }

    const setChart = function () {
        const ctx = document.getElementById('myChart');
        const stats = Game.Stats.getStats(configMap.spel.bord);
        let chart = Game.Stats.getChart(ctx);

        let aantalWit = stats.aantalWit;
        let aantalZwart = stats.aantalZwart;
        chart.data.datasets[0].data = [aantalWit, aantalZwart];
        chart.update();
    }


    const setSpel = async function () {
        await Game.Api.get(`getSpel`, configMap.spelToken).then(json => {

            configMap.spel.bord = json.bord;
            configMap.spel.speler1Token = json.speler1Token;
            configMap.spel.speler2Token = json.speler2Token;
            configMap.spel.aandeBeurt = json.aandeBeurt;
        });
        configMap.speler.color = getColorFromPlayerToken(configMap.speler.token);
        document.getElementById('spelerKleur').innerText = `Jij bent: ${configMap.speler.color}`;
        document.getElementById('beurt').innerText = `${configMap.spel.aandeBeurt === 1 ? "Wit" : "Zwart"} is aan de beurt.`;
    }

    const setSquare = function (i, j) {
        let square = document.createElement('div');
        $(square).addClass('square');
        $(square).attr('data-x', i);
        $(square).attr('data-y', j);
        $(square).click(() => {
            let kleur = configMap.speler.color === "wit" ? 1 : 2;
            if (configMap.spel.aandeBeurt === kleur) {
                Game.Api.put('zet', configMap.spelToken, {rijZet: i - 1, kolomZet: j - 1, pas: false}).then(json => {
                    if (json) {
                        updateBoard();
                    }
                });
            }
        });


        switch(configMap.spel.bord[i-1][j-1]){
            case 1:
            placeFiche(square, configMap.colors[1], i, j);
            break;
            case 2:
            placeFiche(square, configMap.colors[2], i, j);
            break;
        }
        return square;
    }


    const placeFiche = function(square,color, i, j) {
        //add fiche
        if(!square.hasChildNodes()) {
            let fiche = document.createElement('div');
            $(fiche).addClass(`fiche-${color}`);
            $(fiche).attr('data-x', i);
            $(fiche).attr('data-y', j);

            let img = document.createElement('div');
            $(img).addClass('fiche-img');
            fiche.appendChild(img);
            square.appendChild(fiche);
        }
    }


    const getColorFromPlayerToken = function (token) {
        let color = "";
        if (token === configMap.spel.speler1Token) {
            color = configMap.colors[1];
        } else {
            color = configMap.colors[2];
        }
        return color;
    }


    const updateBoard = function () {
        //get the board
        let bord = configMap.spel.bord;
        //return board from the api
        Game.Api.get(`getSpel`, configMap.spelToken).then(json => {
            for (let i = 1; i <= bord.length; i++) {
                for (let j = 1; j <= bord.length; j++) {
                    /**Check if the square has been changed**/
                    let square = document.querySelector(`[data-x="${i}"][data-y="${j}"]`);
                    //if the square has been changed, update the square
                    if (json.bord[i - 1][j - 1] !== bord[i - 1][j - 1]) {
                        //remove fiche
                        if (square.hasChildNodes()) {
                            square.removeChild(square.firstChild);
                        }
                        //place fiche
                        switch (json.bord[i - 1][j - 1]) {
                            case 1:
                                placeFiche(square, configMap.colors[1], i, j);
                                break;
                            case 2:
                                placeFiche(square, configMap.colors[2], i, j);
                                break;
                        }
                    }
                }
            }
            //update the boarddata
            setSpel();

            //update the stats
            setStats();

            //update the chart
            setChart();

            //check if the game is over
            isAfgelopen(json.bord);
        }).catch(() => calculatePoints_AndEndGame(configMap.spel.bord));
    }


    const isAfgelopen = function (bord) {
        //get a true or false from the api
        Game.Api.get(`isAfgelopen`, configMap.spelToken).then(async json => {
                if (json) {
                    //show alert who won by counting the count of stones
                    await calculatePoints_AndEndGame(bord);
                } else {
                    //check if one of the colors is 0
                    if (countFichesByColor("Wit", bord) === 0 || countFichesByColor("Zwart", bord) === 0) {
                        let loser = countFichesByColor("Wit", bord) === 0 ? "Zwart" : "Wit";
                        let winner = countFichesByColor("Wit", bord) === 0 ? "Wit" : "Zwart";
                        configMap.afgelopen = true;
                        await alert(`${loser} heeft geen fiches meer dus ${winner} heeft gewonnen!!`);
                    }
                }
            }
        );
    }

    async function calculatePoints_AndEndGame(bord) {
        await alert(`${countFichesByColor("Wit", bord) > countFichesByColor("Zwart", bord) ? "Wit" : "Zwart"} heeft gewonnen!`);
        window.location.href = "/"; //TODO
        configMap.afgelopen = true;
        configMap.spelToken = "";
        configMap.speler = [];
        configMap.spel = [];
    }

    return {
        initReversi: privateInit,
        updateBoard: updateBoard,
    }
})();

Game.Api = (function () {

    const apiUrl = Game.Data.configMap.apiUrl;

    const getCatFacts = function () {
        return fetch('https://meowfacts.herokuapp.com/')
            .then(res => res.json());
    }

    const get = function (url, spelToken, params) {

        let completeUrl = apiUrl + url;
        return fetch(encodeURI(completeUrl), {
            method: "GET",
            mode: "cors",
            headers: {
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
                "x-speltoken": spelToken
            },
        }).then(res => res.json());
    }
    const put = function (url, spelToken, params) {

            let completeUrl = apiUrl + url;
            console.log('parameters: ', params);
            return fetch(encodeURI(completeUrl), {
                method: "PUT",
                mode: "cors",
                headers: {
                    "Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*",
                    "x-speltoken": spelToken
                },
                body: JSON.stringify(params)
            }).then(res => res.json());
    }

    return {
        get: get,
        put: put,
        getCatFacts: getCatFacts
    }
})();

/*$(function() { //wachtfunctie
console.log( "ready!" );

});*/
class FeedbackWidget{
    constructor(elementId) {
       this._elementId = elementId;
     }

     get elementId() { //getter, set keyword voor setter methode
         return this._elementId;
       }

       show(message, type){//message = succes met... //type = success
            let x = $(`#${this.elementId}`);
            x.removeClass("fade-in");
            x.addClass('fade-out');

            setInterval(() => {
                x.css("display",x.css("display") === "none" && "block");
                x.text(message);

           switch (type) {
               case "success":
                   x.removeClass('fade-out');
                   x.addClass("fade-in alert-success")
                   break;
               case "danger":
               default:
                   x.removeClass('fade-out');
                   x.addClass("fade-in alert-danger")
                   break;
           }
            }, 2000);
           this.log({
               message: message,
               type: type
           });
    };

       hide(){
           let div = $(`#${this.elementId}`);
           div.css("display", div.css("display") === "block" && "none");
   };

    log(message){
        if (localStorage.getItem("JanPietKlaas") === null) {

            let item = {
                messages: [message]
            }
            localStorage.setItem('JanPietKlaas', JSON.stringify(item))
        } else {
            let item = JSON.parse(localStorage.getItem('JanPietKlaas'));
            item.messages.unshift(message);

            if(item.messages.length > 10)
                item.messages.pop();
            localStorage.setItem('JanPietKlaas', JSON.stringify(item));
        }
    }
      removeLog(){
          localStorage.clear();
      }

    history(){
        let item = JSON.parse(localStorage.getItem('JanPietKlaas'));
        let string = "";
        item.messages.forEach(element => {
            string = string + element.type +" - " + element.message + " \n "
        });
        console.log(string);
    }
}
Game.Buttons = (function () {
    const addButtons = function (spelToken) {
        let buttons = document.getElementById('buttons');

        /** ------ Pas de beurt --------- **/
        let pas = document.createElement('button');
        pas.innerHTML = "Pas beurt";
        $(pas).addClass('btn btn-primary');
        pas.addEventListener('click', () => {
            Game.Api.put(`pasBeurt/${spelToken}`, Game.Data.configMap.spelToken).then(json => {
                if (json) Game.Reversi.updateBoard();
                else alert("Je kan je beurt niet passen: er is nog een zet mogelijk.");
            });
        }
        );
        buttons.appendChild(pas);

        /** ------ Verander de catfact --------- **/
        let image = document.createElement('button');
        image.innerHTML = "Verander van catfact";
        $(image).addClass('btn btn-outline-info');
        image.addEventListener('click', () => {
            window.location.reload();
        });
        buttons.appendChild(image);
    }
    return {
        addButtons: addButtons
    }
})();

Game.Stats = (function () {
    const configMap = {
        stats: {
            aantalWit: 0,
            aantalZwart: 0,
            witIsWinning: false,
            aantalLegeVelden: 0,
        }
    }
    const getStats = function (bord) {
        configMap.stats.aantalWit = countFichesByColor("Wit", bord);
        configMap.stats.aantalZwart = countFichesByColor("Zwart", bord);
        configMap.stats.aantalLegeVelden = (bord.length * bord.length) - configMap.stats.aantalWit - configMap.stats.aantalZwart;
        configMap.stats.witIsWinning = configMap.stats.aantalWit > configMap.stats.aantalZwart;
        return configMap.stats;
    }

    const getChart = function (ctx) {
        return new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Wit', 'Zwart'],
                datasets: [{
                    label: 'Aantal fiches',
                    data: [2, 2],
                    backgroundColor: [
                        'rgba(255, 255, 255, 1)',
                        'rgba(0, 0, 0, 1)',
                    ],
                    borderColor: [
                        'rgba(255, 255, 255, 1)',
                        'rgba(0, 0, 0, 1)',
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    const countFichesByColor = (color, bord) => {
        let wit = 0;
        let zwart = 0;
        for (let i = 0; i < bord.length; i++) {
            for (let j = 0; j < bord.length; j++) {
                if (bord[i][j] === 1) {
                    wit++;
                } else if (bord[i][j] === 2) {
                    zwart++;
                }
            }
        }
        return color === "Wit" ? wit : zwart;
    }

    return {
        getStats: getStats,
        getChart: getChart,
        countFichesByColor: countFichesByColor
    }
})();

Game.Template = (function () {
    const getTemplate = function (templateName) {
        let template = spa_templates.templates;
        const templateParts = templateName.split('.');
        for (let templatePart of templateParts) {
            template = template[templatePart];
        }
        return template;
    }

    const parseTemplate = (templateName, data) => {
        return getTemplate(templateName)(data);
    }
    return {
        parseTemplate: parseTemplate
    }
})();
