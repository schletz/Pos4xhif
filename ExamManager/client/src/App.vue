<template>
  <nav>
    <div class="nav-button"><router-link to="/schedule">Schedule</router-link></div>
  </nav>
  <main>
    <router-view></router-view>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
//import HelloWorld from './components/HelloWorld.vue';

export default defineComponent({
  name: 'App',
  components: {
    //HelloWorld
  }
});
</script>

<style>
  nav {
    display: flex;
    flex: 0 1 3rem;
  }

  nav > div {
    cursor: pointer;
    padding: 0.5rem;
    border:1px solid var(--main-color);
  }
  main {
    flex-grow: 1;
  }
</style>
