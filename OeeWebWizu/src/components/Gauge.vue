<template>
  <div class="gauge">
    <div class="radialProgressBar">
      <CircleProgress
       :percent="rateValue < 100 ? rateValue : 100"
       :fill-color="getValueColor(rateValue)"
       :size="circleSize"
       :border-width="borderWidth"
       :empty-color="emptyColor"/>
    </div>
    <div class="oeeValue" :style="{color: getValueColor(rateValue)}">{{rateValue}}%</div>
    <h1 class="oeeDesc">{{rateName}}</h1>
  </div>
</template>

<script>
import "vue3-circle-progress/dist/circle-progress.css";
import CircleProgress from "vue3-circle-progress";
export default ({
  data () {
    return {
      completedSteps: 0,
      circleSize: 400,
      borderWidth: 20,
      emptyColor: "rgb(36, 36, 36)"
    }
  },
  setup() {
    
  },
  methods: {
    getValueColor(value){
      if(value <= 20){
        return "#ff1a1a";
      }
      else if(value > 20 && value <= 60){
        return "#ffab1a";
      }
      else{
        return "#29ff15";
      }
    }
  },
  props: [
    'rateValue',
    'rateName'
  ],
  components: {
    CircleProgress 
  }
})
</script>


<style scoped>
  div.oeeValue {
    position: absolute;
    height: 10vh;
    width: 15vw;
    top: 80px;
    left: 80px;
    font-size: 124px;
    font-weight: 600;
  }

  h1.oeeDesc {
    position: absolute;
    height: 10vh;
    width: 15vw;
    top: 180px;
    left: 60px;
    font-size: 56px;
    font-weight: 600;
  }

  div.radialProgressBar {
    position: absolute;
    left: 20px;
  }
</style>