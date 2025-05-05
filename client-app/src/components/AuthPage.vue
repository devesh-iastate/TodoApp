<template>
  <div class="auth-container">
    <div class="tabs">
      <button :class="{ active: isLogin }" @click="isLogin = true">Login</button>
      <button :class="{ active: !isLogin }" @click="isLogin = false">Sign Up</button>
    </div>

    <h1>{{ isLogin ? 'Login' : 'Sign Up' }}</h1>

    <form @submit.prevent="handleAuth">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">{{ isLogin ? 'Login' : 'Sign Up' }}</button>
    </form>
  </div>
</template>

<script>
import { auth } from '@/services/firebase';
import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword
} from 'firebase/auth';

export default {
  name: 'AuthPage',
  data() {
    return {
      email: '',
      password: '',
      isLogin: true
    };
  },
  methods: {
    async handleAuth() {
      try {
        if (this.isLogin) {
          await signInWithEmailAndPassword(auth, this.email, this.password);
        } else {
          await createUserWithEmailAndPassword(auth, this.email, this.password);
        }
        this.$emit('auth-success');
      } catch (err) {
        alert(err.message);
        console.error('Authentication Error:', err);
      }
    }
  }
};
</script>

<style scoped>
.auth-container {
  max-width: 400px;
  margin: auto;
  padding: 2rem;
  border: 1px solid #ccc;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.tabs {
  display: flex;
  justify-content: space-around;
  margin-bottom: 1rem;
}

.tabs button {
  padding: 0.5rem 1rem;
  background-color: #f3f4f6;
  border: none;
  cursor: pointer;
  border-radius: 8px;
  font-weight: 500;
}

.tabs button.active {
  background-color: #3b82f6;
  color: white;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}

button[type="submit"] {
  width: 100%;
  padding: 0.75rem;
  background-color: #3b82f6;
  color: white;
  font-weight: bold;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}

button[type="submit"]:hover {
  background-color: #2563eb;
}
</style>