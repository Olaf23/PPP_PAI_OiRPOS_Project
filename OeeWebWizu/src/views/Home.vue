<template>
  <div class="home">
    <div id="nav">
      <router-link to="/">Home</router-link> |
      <router-link to="/about">About</router-link> |
      <router-link to="/visualization">Visualization</router-link>
    </div>
    <img alt="Vue logo" src="../assets/logo.png" ><br/>
    <button class="logoutButton" @click="logOutMesApi">Wyloguj</button>
  </div>
</template>

<script>
// @ is an alias to /src
import axios from 'axios';

export default {
  name: 'Home',
  data() {
    return {
      testVar: 1
    }
  },
  methods:{
    async logOutMesApi(){
      try{
        await axios.get('http://localhost:44379/api/user/LogOutMES', {
          withCredentials: true
          });
        document.cookie = "AccessToken=none";
        sessionStorage.setItem('isUserLogged', false);
        this.$router.push('login');
      }
      catch (e){
        console.error(e);
        alert(e);
      }
    }
  }
}
</script>

<style scoped>
  div.home {
    background: rgb(225,111,22);
    background: linear-gradient(45deg, rgba(255,112,0,1) 0%, rgba(225,111,22,1) 20%, rgba(255,112,0,1) 100%);
    height: 100vh;
  }
</style>