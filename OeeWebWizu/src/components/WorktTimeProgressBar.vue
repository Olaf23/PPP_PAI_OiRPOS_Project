<template>
  <div class="worktTimeProgressBar">
      <div class="hourLine">
        <span class="hour">{{createLabelForHour(shiftNumber, 1)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 2)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 3)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 4)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 5)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 6)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 7)}}</span>
        <span class="hour">{{createLabelForHour(shiftNumber, 8)}}</span>
      </div>
      <span class="timeBlock"
        v-for="workTime in oeeWorkTimesCollection"
        :key="workTime.Id"
        :style="{
            width: getWidthFromDuration(workTime.duration),
            background: getColorForWorkTime(workTime.workStatus)
            }"/>
  </div>
</template>

<script>

export default ({
  setup() {
    
  },
  props: ['oeeWorkTimesCollection',"shiftNumber"],
  methods:{
      getWidthFromDuration(duration){
          //var winWidth = window.screen.width - 20;
          var durPropotion = duration / (8 * 60 * 60);
          return (durPropotion * 100).toString() + 'vw';
      },
      getColorForWorkTime(workType){
        if(workType == 30){
            return 'green';
        }
        else if (workType == 40){
            return 'orange';
        }
        else {
            return 'red';
        }
      },
      createLabelForHour(shiftNum, hourNumber){
        if(shiftNum == 2){
            return (hourNumber + 14).toString() + ":00";
          
        }
        else if(shiftNum == 2){
          if(hourNumber > 2)
            return "0" + (hourNumber - 2).toString() + ":00";
          else
            return (hourNumber + 22).toString() + ":00";
        }
        else{
          if(hourNumber < 4)
            return "0" + (hourNumber + 6).toString() + ":00";
          else
            return (hourNumber + 6).toString() + ":00";
        }
      }
  }
})
</script>


<style scoped>
  div.hourLine{
    height: 5vh;
    position: absolute;
    bottom: 3vh;
  }

  span.hour {
    width: 12.5vw;
    display: inline-block;
    position: relative;
    text-align: right;
    right: 5px;
    border-color: #dcdcdc;
    border-width: 3px;
  }
  
  span.timeBlock {
    display: inline-block;
    top: 5vh;
    height: 5vh;
    position: relative;
  }
</style>