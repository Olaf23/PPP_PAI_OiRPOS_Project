<template>
  <div class="hourSummaryRow">
      <span class="lineHour"> {{getHourName(hourPartSummary.hour)}} </span>
      <span class="line"> {{hourPartSummary.okCount}} </span>
      <span class="line"> {{hourPartSummary.nokCount}} </span>
      <span class="line"> {{hourPartSummary.target}} </span>
  </div>
</template>

<script>

export default ({
  setup() {
    
  },
  props:[
    'shiftNumber',
    'hourPartSummary'
  ],
  methods:{
    getHourName(hourNumber){
      if(hourNumber == -1){
        if(this.shiftNumber == 1)
          return "06:00 - 14:00";
        else if(this.shiftNumber == 2)
          return "14:00 - 22:00";
        else
          return "22:00 - 06:00";
      }
      if(hourNumber < 9)
        return "0" + (hourNumber).toString() + ":00 - 0" + (hourNumber + 1).toString() + ":00";
      else if (hourNumber == 9)
        return "0" + (hourNumber).toString() + ":00 - " + (hourNumber + 1).toString() + ":00";
      else
        return (hourNumber).toString() + ":00 - " + (hourNumber + 1).toString() + ":00";
    }
  }
})
</script>


<style scoped>
  span.line {
    display: inline-block;
    width: 10vw;
    height: 7vh;
    font-size: 40px;  
    font-weight: 800;
  }
  
  span.lineHour {
    display: inline-block;
    width: 20vw;
    height: 7vh;
    font-size: 40px;  
    font-weight: 800;
  }
</style>