<template>
  <div>
    <h2>To-Do List ({{ provider }})</h2>

    <input v-model="searchingTerm" placeholder="Search..." />

    <div v-if="isFetching" class="spinner">Loading your todos...</div>

    <table v-else>
      <thead>
      <tr>
        <th style="width: 10%;">Done</th>
        <th style="width: 30%;">Title</th>
        <th style="width: 40%;">Description</th>
        <th style="width: 20%;">Actions</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="todo in todos" :key="todo.id">
        <td>
          <input type="checkbox" v-model="todo.isComplete" @change="markAsDone(todo)" />
        </td>

        <td v-if="editingId === todo.id">
          <input v-model="editedTitle" style="width: 100%;" />
        </td>
        <td v-else :style="{ textDecoration: todo.isComplete ? 'line-through' : 'none' }">
          {{ todo.title }}
        </td>

        <td v-if="editingId === todo.id">
          <input v-model="editedDescription" style="width: 100%;" />
        </td>
        <td v-else>
          {{ todo.description }}
        </td>

        <td>
          <template v-if="editingId === todo.id">
            <button @click="saveEdit(todo)" :disabled="editedTitle.trim().length === 0">Save</button>
            <button @click="cancelEdit">Cancel</button>
          </template>
          <template v-else>
            <button @click="startEdit(todo)">Edit</button>
            <button @click="removeTodo(todo.id)">Delete</button>
          </template>
        </td>
      </tr>

      <tr>
        <td></td>
        <td>
          <input v-model="newTodoTitle" placeholder="New to-do title" style="width: 100%;" />
        </td>
        <td>
          <input v-model="newTodoDescription" placeholder="New to-do description" style="width: 100%;" />
        </td>
        <td>
          <button @click="createTodo" :disabled="newTodoTitle.trim() === ''">Add To-Do</button>
        </td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import api from '@/services/api';
import _ from 'lodash';
import Todo from '@/models/Todo';

export default {
  name: 'TodoList',
  props: ['provider'],
  data() {
    return {
      todos: [],
      newTodoTitle: '',
      newTodoDescription: '',
      searchingTerm: '',
      editedTitle: '',
      editedDescription: '',
      editingId: null,
      isFetching: false,
      delayedFetch: null,
      lastUpdated: null,
      pollInterval: null
    };
  },
  created() {
    this.delayedFetch = _.debounce(() => {
      this.loadTodos();
    }, 300);

    this.loadTodos();
  },
  watch: {
    searchingTerm() {
      this.delayedFetch();
    },
    provider() {
      this.loadTodos();
    }
  },
  methods: {
    async pollUpdates() {
      try {
        const res = await api.get(`/todos/${this.provider}/last-updated`);
        const serverTime = res.data;
        if (this.lastUpdated !== serverTime) {
          this.loadTodos();
          this.lastUpdated = serverTime;
        }
      } catch (err) {
        console.error('Polling error:', err);
      }
    },
    loadTodos() {
      this.isFetching = true;
      api.get(`/todos/${this.provider}`, {
        params: {
          search: this.searchingTerm
        }
      })
          .then(result => {
            this.todos = result.data.todos.map(item => new Todo(item));
            this.lastUpdated = result.data.lastUpdated;
          })
          .catch(err => {
            console.error("Could not get todos:", err);
          })
          .finally(() => {
            this.isFetching = false;
          });
    },
    createTodo() {
      if (this.newTodoTitle.trim().length === 0) return;

      const todo = new Todo({
        title: this.newTodoTitle.trim(),
        description: this.newTodoDescription.trim()
      });

      api.post(`/todos/${this.provider}`, todo)
          .then(() => {
            this.newTodoTitle = '';
            this.newTodoDescription = '';
            this.loadTodos();
          })
          .catch(console.error);
    },
    startEdit(todo) {
      this.editingId = todo.id;
      this.editedTitle = todo.title;
      this.editedDescription = todo.description;
    },
    cancelEdit() {
      this.editingId = null;
      this.editedTitle = '';
      this.editedDescription = '';
    },
    saveEdit(todo) {
      const updatedTodo = new Todo({
        id: todo.id,
        title: this.editedTitle,
        description: this.editedDescription,
        isComplete: todo.isComplete
      });

      api.put(`/todos/${this.provider}/${todo.id}`, updatedTodo)
          .then(() => {
            this.cancelEdit();
            this.loadTodos();
          })
          .catch(console.error);
    },
    markAsDone(todo) {
      const updated = new Todo(todo);
      api.put(`/todos/${this.provider}/${todo.id}`, updated)
          .catch(console.error);
    },
    removeTodo(id) {
      api.delete(`/todos/${this.provider}/${id}`)
          .then(() => {
            this.loadTodos();
          })
          .catch(console.error);
    }
  },
  mounted() {
    this.pollInterval = setInterval(this.pollUpdates, 1000);
  },
  beforeUnmount() {
    clearInterval(this.pollInterval);
  }
};
</script>

<style scoped>
input[type="text"],
input[type="search"] {
  padding: 6px;
  margin: 5px;
  width: 250px;
}
button {
  margin-left: 5px;
  padding: 6px 10px;
  font-size: 14px;
}
table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
}
th, td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}
thead {
  background-color: #f9f9f9;
}
.spinner {
  margin: 20px 0;
  font-weight: bold;
  color: #555;
}
</style>