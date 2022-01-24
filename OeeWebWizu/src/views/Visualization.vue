<template>
  <div class="visualization">
    <!-- <div class="stationSelector">
    <select @change="onChange">
        <option v-for="rating in oeeRatingCollection" :key="rating.idStation" :value="rating.idStation">{{rating.idStation}}</option>
    </select>
    </div> -->

    <div class="stationTitle">{{selectedStationName}}</div>

    <div class="clock">{{timeNow}}</div>

    <div class="oeeGauge">
      <Gauge :rateValue="oeeRate" rateName="OEE"/>
    </div>

    <div class="avGauge">
      <Gauge :rateValue="availabilityRate" rateName="Dostępność"/>
    </div>

    <div class="perGauge">
      <Gauge :rateValue="performanceRate" rateName="Wydajność"/>
    </div>

    <div class="quaGauge">
      <Gauge :rateValue="qualityRate" rateName="Jakość"/>
    </div>

    <div class="shiftSummaryPartsTable">
      <ShiftSummaryPartsTable
       :oeeHourSummariesCollection="oeeHourSummariesCollectionForStation"
       :oeeShiftSummaryForStation="oeeShiftSummaryForStation"
       :shiftNumber="shiftNumber"
       /><br/>
    </div>

    <div class="breakSum"> Suma czasu awarii: <br/> {{getBreakSum()}} </div>

    <div class="workStatus"> Aktualny status pracy: <br/> {{getCurrentWorkTimeStatus()}} </div>

    <div class="worktTimeProgressBar">
      <WorktTimeProgressBar
       :oeeWorkTimesCollection="oeeWorkTimesCollectionForStation"
       :shiftNumber="shiftNumber"
        align="left"/>
    </div>
  </div>
</template>

<script>
import Gauge from '@/components/Gauge.vue'
import ShiftSummaryPartsTable from '@/components/ShiftSummaryPartsTable.vue'
import WorktTimeProgressBar from '@/components/WorktTimeProgressBar.vue'
import axios from 'axios';

export default {
  components: {
    Gauge,
    ShiftSummaryPartsTable,
    WorktTimeProgressBar
  },
    data() {
    return {
      requestTest: '',
      oeeRatingCollection: [{idArea: -1, idStation: -1, oeeRate: 0, availabilityRate: 0, performanceRate: 0, qualityRate: 0}],
      oeeRatingCollectionForStation: {idArea: -1, idStation: -1, oeeRate: 0, availabilityRate: 0, performanceRate: 0, qualityRate: 0},
      oeeHourSummariesCollection: [{idArea: -1, idStation: -1, hour: -1, okCount: -1, nokCount: -1, target: -1}],
      oeeHourSummariesCollectionForStation: [{idArea: -1, idStation: -1, hour: 1, okCount: -1, nokCount: -1, target: -1}],
      oeeShiftSummaryForStation: {idArea: -1, idStation: -1, hour: 1, okCount: -1, nokCount: -1, target: -1},
      oeeWorkTimesCollection: [],
      oeeWorkTimesCollectionForStation: [],
      oeeRate: 0,
      availabilityRate: 0,
      performanceRate: 0,
      qualityRate: 0,
      selectedIdStation: -1,
      shiftNumber: 1,
      timeNow: "00:00:00",
      stations: [{id: -1, idArea: -1, name: ""}],
      selectedStationName: ""
    }
  },
  created: function (){
      this.getProductionStructure();
      this.oeeResultLoop();
      setInterval(this.showTime, 1000);
    },
  methods:{
    someAction(){
      alert('some action was done')
    },
    oeeResultLoop: function(){
      var ref = this;
      setTimeout(async function() {
        await ref.getOeeResult();
        ref.oeeResultLoop();
      }, 1000);
    },
    getOeeResult: async function(){
      var ref = this;
      try{
        const res = await axios.get('http://localhost:44379/oee/visualization/GetOeeResult', {
          withCredentials: true
          });
        var oeeResult = res.data;
        this.oeeRatingCollection = oeeResult.oeeRatingsCollection;
        this.oeeHourSummariesCollection = oeeResult.oeeHourSummariesCollection;
        this.oeeWorkTimesCollection = oeeResult.oeeWorkTimesCollection;
        ref.getActualShiftNumber();
        ref.updateOeeControls();
      }
      catch (e){
        console.error(e);
      }
    },
    getProductionStructure: async function(){
      try{
        const res = await axios.get('http://localhost:44379/oee/visualization/GetProductionStructure', {
          withCredentials: true
          });
        var prodStruct = res.data;
        this.stations = prodStruct.stations;
        this.selectedIdStation = this.stations[0].id;
        this.selectedStationName = this.stations[0].name;
      }
      catch (e){
        console.error(e);
      }
    },
    getBreakSum(){
      var ref = this;
      let breaks = this.oeeWorkTimesCollectionForStation.filter(x=>x.workStatus == 20);
      let breaksDurationSum = 0;
      for ( var i = 0; i < breaks.length; i++ ) {
        breaksDurationSum += breaks[i].duration;
      }
      return ref.getTimeStringFormat(breaksDurationSum);
    },
    getTimeStringFormat(seconds){
      let hh = Math.floor(seconds/3600);
      let mm = Math.floor((seconds/60)%60);
      let ss = Math.floor(seconds%60);
      return (hh > 0 ? hh.toString() + " godz. " : "") + (mm + hh > 0 ? mm.toString() + " min. " : "") + ss.toString() + " s.";
    },
    getCurrentWorkTimeStatus: function(){
      try{
        let len = this.oeeWorkTimesCollectionForStation.length;
        let workStatus = this.oeeWorkTimesCollectionForStation[len - 1].workStatus;
        if (workStatus == 40){
          return "Przerwa";
        }
        else if (workStatus == 30){
          return "Praca";
        }
        else{
          return "Awaria";
        }
      }
      catch{
          return "Brak";
      }
    },
    updateOeeControls(){
      var ratingsForStation = this.oeeRatingCollection.find(x=>x.idStation == this.selectedIdStation);
      this.oeeRatingCollectionForStation = ratingsForStation;
      this.oeeRate = Math.round(this.oeeRatingCollectionForStation.oeeRate);
      this.availabilityRate = Math.round(this.oeeRatingCollectionForStation.availabilityRate);
      this.performanceRate = Math.round(this.oeeRatingCollectionForStation.performanceRate);
      this.qualityRate = Math.round(this.oeeRatingCollectionForStation.qualityRate);
      this.oeeHourSummariesCollectionForStation = this.oeeHourSummariesCollection.filter(x=>x.idStation == this.selectedIdStation);
      this.oeeShiftSummaryForStation = this.oeeHourSummariesCollection.filter(x=>x.idStation == this.selectedIdStation && x.hour == -1)[0];
      this.oeeWorkTimesCollectionForStation = this.oeeWorkTimesCollection.filter(x=>x.idStation == this.selectedIdStation);
    },
    onChange(event) {
      var ref = this;
      this.selectedIdStation = event.target.value;
      ref.updateOeeControls();
    },
    getActualShiftNumber(){
      let curHour = new Date().getHours();
      if(curHour >= 6 && curHour < 14){
        this.shiftNumber = 1;
      }
      else if(curHour >= 14 && curHour < 22){
        this.shiftNumber = 2;
      }
      else{
        this.shiftNumber = 3;
      }
    },
    showTime: function () {
      let time = new Date();
      let hour = time.getHours();
      let min = time.getMinutes();
      let sec = time.getSeconds();

      hour = hour < 10 ? "0" + hour : hour;
      min = min < 10 ? "0" + min : min;
      sec = sec < 10 ? "0" + sec : sec;

      let currentTime = hour + ":" + min + ":" + sec;

      this.timeNow = currentTime;
    }
  }
}
</script>

<style scoped>
  div.visualization {
    background-image: linear-gradient(to right bottom, #252525, #202020, #1c1c1c, #171717, #121212);
    height: 100vh;
    width: 100vw;
  }

  div.stationSelector {
    position: fixed;
    height: 5vh;
    width: 5vw;
    top: 5vh;
    left: 50vw;
    font-size: 24px;
  }

  div.stationTitle {
    position: fixed;
    height: 10vh;
    width: 35vw;
    left: 50vw;
    font-size: 64px;
    font-weight: 600;
  }

  div.clock {
    position: fixed;
    height: 10vh;
    width: 15vw;
    top: 0vh;
    left: 85vw;
    font-size: 64px;
    font-weight: 600;
  }

  div.oeeGauge {
    position: fixed;
    height: 22.5vh;
    width: 25vw;
    top: 2vh;
    left: 0vw;
  }

  div.avGauge {
    position: fixed;
    height: 20vh;
    width: 25vw;
    top: 2vh;
    left: 25vw;
  }

  div.perGauge {
    position: fixed;
    height: 20vh;
    width: 25vw;
    top: 40vh;
    left: 0vw;
  }

  div.quaGauge {
    position: fixed;
    height: 20vh;
    width: 25vw;
    top: 40vh;
    left: 25vw;
  }

  div.shiftSummaryPartsTable {
    position: fixed;
    height: 72vh;
    width: 50vw;
    top: 10vh;
    left: 50vw;
  }

  div.breakSum {
    position: fixed;
    height: 10vh;
    width: 50vw;
    top: 80vh;
    left: 0vw;
    font-size: 48px;
    font-weight: 600;
  }

  div.workStatus {
    position: fixed;
    height: 10vh;
    width: 50vw;
    top: 80vh;
    left: 50vw;
    font-size: 48px;
    font-weight: 600;
  }

  div.worktTimeProgressBar {
    position: fixed;
    height: 10vh;
    width: 100vw;
    top: 90vh;
    left: 0vw;
  }
</style>