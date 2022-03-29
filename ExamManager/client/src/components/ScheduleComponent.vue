<template>
    <p>{{ exams.length }} exams found!</p>
    <ul>
        <li v-for="e in exams" :key="e.guid">
            {{ e.guid }}
        </li>
    </ul>
</template>

<script>
import { defineComponent } from 'vue';
import config from '../config.js';

export default defineComponent({
    data() {
        return {
            exams: [],
        };
    },
    async mounted() {
        const response = await fetch(`${config.apiUrl}/api/exams`);
        this.exams = await response.json();
    },
});
</script>
