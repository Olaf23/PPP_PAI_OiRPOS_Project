<template>
  <div class="loginPage">
    <div class="loginPanel">
        <div class="grid-container">
        <div class="grid-item">Login:</div>
        <div class="grid-item"><input type="text" placeholder="login" v-model="userLogin"/></div>
        <div class="grid-item">Hasło:</div>  
        <div class="grid-item"><input type="password" placeholder="hasło" v-model="userPassword"/></div>
        <div class="grid-item"><button class="loginButton" @click="logIntoMesApi">Zaloguj</button></div>
        </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
//document.cookie = "AccessToken=AjumhNNeBog3HTdtYEJIktIWl7CGoSZos5vXAeaJmMqh2AkZX1BFkHmLBHO6SL9L";
export default {
  name: 'HelloWorld',
  data() {
    return {
      userLogin: '',
      userPassword: ''
    }
  },
  methods:{
    async logIntoMesApi(){
      try{
        var tryingToLogUser = {login: this.userLogin, password: this.userPassword, isPasswordHashed: false };
        const res = await axios.put('http://localhost:44379/api/user/LoginIntoMES', tryingToLogUser, {
          withCredentials: true
          });
        var resUser = res.data;
        if (resUser)
        {
          document.cookie = "AccessToken="+resUser.token;
          sessionStorage.setItem('isUserLogged', true);
          this.$router.push('/');
        }
      }
      catch (e){
        console.error(e);
        alert("Nie udało się zalogować\nError: " + e);
      }
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  div.loginPage {
    background: rgb(225,111,22);
    background: linear-gradient(45deg, rgba(255,112,0,1) 0%, rgba(225,111,22,1) 20%, rgba(255,112,0,1) 100%);
    height: 100vh;
  }
  .grid-container {
    display: grid;
    grid-template-columns: auto;
    padding: 0px;
    width: 300px;
  }

  .grid-item {
    padding-top: 8px;
    padding-bottom: 8px;
    width: 300px;
    text-align: left;
    font-size:24px;
    font-weight: bold;
    color: #dcdcdc;
  }
  div.loginPanel{
    position: absolute;
    height: 300px;
    width: 300px;
    -ms-transform: translate(-50%, -50%);
    transform: translate(-50%, -50%);
    top: 50%;
    left: 50%;
  }
  input[type="text"]
  {
    font-size:24px;
    background-color: #dcdcdc;
    padding: 0;
    margin: 0;
    width: 300px;
    height: 50px;
    border-width: 0px;
  }
  input[type="password"]
  {
    font-size:24px;
    background-color: #dcdcdc;
    padding: 0;
    margin: 0;
    width: 300px;
    height: 50px;
    border-width: 0px;
  }
  button.loginButton{
    background-color: transparent;
    cursor: pointer;
    color: #dcdcdc;
    width: 300px;
    height: 50px;
    margin-top: 20px;
    border-width: 0px;
    font-size:24px;
    font-weight: bold;
  }
</style>
