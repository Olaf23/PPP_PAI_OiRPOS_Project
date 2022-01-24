<template>
  <div class="shiftSummaryPartsTable">
      <span class="lineHour"> Godzina </span>
      <span class="line"> OK </span>
      <span class="line"> NOK </span>
      <span class="line"> Target </span>
      <!-- <HourSummaryRow :hourPartSummary="oeeShiftSummaryForStation" :shiftNumber="shiftNumber"/> -->
      <HourSummaryRow v-for="summaryRow in oeeHourSummariesCollection" :key="summaryRow.hour" :hourPartSummary="summaryRow" :shiftNumber="shiftNumber"/>
  </div>
</template>

<script>
import HourSummaryRow from '@/components/HourSummaryRow.vue'
export default ({
    created: function(){
    },
    components: {
        HourSummaryRow
  },
  setup() {
    
  },
  props: ['oeeHourSummariesCollection', 'oeeShiftSummaryForStation', 'shiftNumber']
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