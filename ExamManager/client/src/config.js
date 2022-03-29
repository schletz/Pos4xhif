const dev = {
    apiUrl: "https://localhost:5001"
};

const production = {
    apiUrl: ""
};

export default process.env.NODE_ENV == 'production' ? production : dev;