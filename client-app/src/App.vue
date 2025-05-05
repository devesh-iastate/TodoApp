<template>
  <div id="app">
    <AuthPage v-if="!isAuthenticated" @auth-success="onAuthSuccess" />
    <div v-else>
      <ProviderSelector @selected="setProvider" />
      <TodoList :provider="selectedProvider" />
      <button @click="logout" class="logout-btn">Logout</button>
    </div>
  </div>
</template>

<script>
import ProviderSelector from "@/components/ProviderSelector.vue";
import TodoList from "@/components/TodoList.vue";
import AuthPage from "@/components/AuthPage.vue";
import { onAuthStateChanged, signOut } from "firebase/auth";
import { auth } from "@/services/firebase";

export default {
  name: "App",
  components: { ProviderSelector, TodoList, AuthPage },
  data() {
    return { selectedProvider: "LocalDb", isAuthenticated: false };
  },
  methods: {
    setProvider(provider) {
      this.selectedProvider = provider;
    },
    onAuthSuccess() {
      this.isAuthenticated = true;
    },
    async logout() {
      await signOut(auth);
      localStorage.removeItem("idToken");
      this.isAuthenticated = false;
    }
  },
  mounted() {
    onAuthStateChanged(auth, async (user) => {
      if (user) {
        const token = await user.getIdToken();
        localStorage.setItem("idToken", token);
        this.isAuthenticated = true;
      }
    });
  }
};
</script>

<style scoped>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  color: #2c3e50;
  margin-top: 40px;
}
.logout-btn {
  margin-top: 20px;
  background-color: #ef4444;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
.logout-btn:hover {
  background-color: #dc2626;
}
</style>